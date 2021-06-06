using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowStats : WeaponStats
{
    [SerializeField]
    LineRenderer laser1, laser2;
    [SerializeField]
    DataTransfer dataTransfer;
    [SerializeField]
    GameObject rotPoint;
    private void Update()
    {
        if (gm.Player.es.IsDead)
        {
            laser.enabled = false;
            return;
        }
        if (IsActive)
        {
            if (!om.isToggleAltFire)
            {
                if (InputHandler.GetAction(gm.gc.Player.AltFireToggle))
                {
                    if (secondaryChip != null)
                    {
                        AltFire = true;
                    }
                }
                else
                {
                    AltFire = false;
                }
            }
            if (!AltFire)
            {
                if (gm.Player.overclock.isBikeRunning)
                {
                    laser1.enabled = false;
                    laser2.enabled = false;
                    return;
                }
                primaryChip.ChipLogic(this);
                if (isTempLaserDisable)
                {
                    laser.enabled = false;
                }
                else if (om.isWeaponLasersEnabled && gm.Player.es.GetEntityState() != EntityStats.EntityStates.Melee)
                {
                    primaryChip.CastWeaponLaser(laser);
                    laser.enabled = true;
                }
                else
                {
                    laser.enabled = false;
                }
            }
            else
            {
                if (gm.Player.overclock.isBikeRunning)
                {
                    laser1.enabled = false;
                    laser2.enabled = false;
                    return;
                }
                secondaryChip.ChipLogic(this);
                if (isTempLaserDisable)
                {
                    laser.enabled = false;
                }
                else if (om.isWeaponLasersEnabled && gm.Player.es.GetEntityState() != EntityStats.EntityStates.Melee)
                {
                    secondaryChip.CastWeaponLaser(laser);
                    laser.enabled = true;
                }
                else
                {
                    laser.enabled = false;
                }
            }
            CastLasers();
        }
        else
        {
            laser.enabled = false;
            laser1.enabled = false;
            laser2.enabled = false;
        }
        if (primaryChip.fireRateCounter < primaryChip.fireRate)
        {
            primaryChip.fireRateCounter += Time.deltaTime;
        }
        if (secondaryChip != null)
        {
            if (secondaryChip.fireRateCounter < secondaryChip.fireRate)
            {
                secondaryChip.fireRateCounter += Time.deltaTime;
            }
        }
    }
    void CastLasers()
    {
        if (isTempLaserDisable)
        {
            laser1.enabled = false;
            laser2.enabled = false;
            return;
        }
        if(secondaryChip == null||!AltFire)
        {
            laser1.enabled = false;
            laser2.enabled = false;
            return;
        }
        if (!secondaryChip.chipClassName.Equals("SplitShotChip"))
        {
            laser1.enabled = false;
            laser2.enabled = false;
            return;
        }
        if (om.isWeaponLasersEnabled)
        {
            laser1.enabled = true;
            laser2.enabled = true;
            float angle = dataTransfer.transferFloats[0];
            //Laser 1
            rotPoint.transform.eulerAngles = new Vector3(0, 0, angle);
            rotPoint.transform.eulerAngles += transform.eulerAngles;
            Vector3 startPos = laser1.gameObject.transform.position + rotPoint.gameObject.transform.up * 0;
            Vector3 endPos = startPos + rotPoint.transform.up * secondaryChip.laserProfile.LaserLength;
            RaycastHit2D hit = Physics2D.Raycast(laser1.transform.position, rotPoint.transform.up, secondaryChip.laserProfile.LaserLength, LayerMask.GetMask("Walls"));
            if (hit)
            {
                endPos = hit.point;
            }
            Vector3 midPos = startPos + rotPoint.transform.up * (Vector3.Distance(startPos, endPos) / 2);
            laser1.SetPosition(0, startPos);
            laser1.SetPosition(1, midPos);
            laser1.SetPosition(2, endPos);
            //Laser 2
            rotPoint.transform.eulerAngles = new Vector3(0, 0, angle * -1);
            rotPoint.transform.eulerAngles += transform.eulerAngles;
            startPos = laser2.gameObject.transform.position + rotPoint.gameObject.transform.up * 0;
            endPos = startPos + rotPoint.transform.up * secondaryChip.laserProfile.LaserLength;
            hit = Physics2D.Raycast(laser2.transform.position, rotPoint.transform.up, secondaryChip.laserProfile.LaserLength, LayerMask.GetMask("Walls"));
            if (hit)
            {
                endPos = hit.point;
            }
            midPos = startPos + rotPoint.transform.up * (Vector3.Distance(startPos, endPos) / 2);
            laser2.SetPosition(0, startPos);
            laser2.SetPosition(1, midPos);
            laser2.SetPosition(2, endPos);
        }
        else
        {
            laser.enabled = true;
            laser1.enabled = true;
            laser2.enabled = true;
            float angle = dataTransfer.transferFloats[0];
            //OG Laser
            Vector3 startPos = laser.gameObject.transform.position + laser.gameObject.transform.up * secondaryChip.laserProfile.StartDistance;
            Vector3 endPos = startPos + laser.transform.up * 3;
            RaycastHit2D hit = Physics2D.Raycast(laser.transform.position, laser.gameObject.transform.up, 3, LayerMask.GetMask("Walls"));
            if (hit)
            {
                endPos = hit.point;
            }
            Vector3 midPos = startPos + laser.gameObject.transform.up * (Vector3.Distance(startPos, endPos) / 2);
            laser.SetPosition(0, startPos);
            laser.SetPosition(1, midPos);
            laser.SetPosition(2, endPos);
            //Laser 1
            rotPoint.transform.eulerAngles = new Vector3(0, 0, angle);
            rotPoint.transform.eulerAngles += transform.eulerAngles;
            startPos = laser1.gameObject.transform.position + rotPoint.gameObject.transform.up * 0;
            endPos = startPos + rotPoint.transform.up * 3;
            hit = Physics2D.Raycast(laser1.transform.position, rotPoint.transform.up, 3, LayerMask.GetMask("Walls"));
            if (hit)
            {
                endPos = hit.point;
            }
             midPos = startPos + rotPoint.transform.up * (Vector3.Distance(startPos, endPos) / 2);
            laser1.SetPosition(0, startPos);
            laser1.SetPosition(1, midPos);
            laser1.SetPosition(2, endPos);
            //Laser 2
            rotPoint.transform.eulerAngles = new Vector3(0, 0, angle * -1);
            rotPoint.transform.eulerAngles += transform.eulerAngles;
            startPos = laser2.gameObject.transform.position + rotPoint.gameObject.transform.up * 0;
            endPos = startPos + rotPoint.transform.up * 3;
            hit = Physics2D.Raycast(laser2.transform.position, rotPoint.transform.up, 3, LayerMask.GetMask("Walls"));
            if (hit)
            {
                endPos = hit.point;
            }
            midPos = startPos + rotPoint.transform.up * (Vector3.Distance(startPos, endPos) / 2);
            laser2.SetPosition(0, startPos);
            laser2.SetPosition(1, midPos);
            laser2.SetPosition(2, endPos);
        }
    }
}
