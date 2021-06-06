using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class WeaponChip : Chip
{
    public int ammoCost = 0;
    [SerializeField]
    protected GameMaster.AmmoTypes ammoType;
    public float fireRate = 0;
    [HideInInspector]
    public float fireRateCounter = 0;
    [SerializeField]
    protected bool isPressInput;
    [SerializeField]
    protected GameObject projectile;
    [SerializeField]
    protected Damage damage = null;
    public LaserProfile laserProfile;
    protected bool isInputReleased;
    protected float shootRaycastLength = 400;
    public bool isMeleeChip;

    public void CopyWeaponChipCoreValues(WeaponChip wc)
    {
        chipID = wc.chipID;
        chipClassName = wc.chipClassName;
        chipSprite = wc.chipSprite;
        ammoCost = wc.ammoCost;
        ammoType = wc.ammoType;
        fireRate = wc.fireRate;
        isPressInput = wc.isPressInput;
        projectile = wc.projectile;
        damage = wc.damage;
        isUnlocked = wc.isUnlocked;
        laserProfile = wc.laserProfile;
        isMeleeChip = wc.isMeleeChip;
    }
    public virtual void ChipLogic(WeaponStats ws)
    {
        if (ws.gm.Player.gameObject.GetComponent<EntityStats>().IsDead)
        {
            return;
        }
        if (ws.gm.um.UIState != UIMaster.UIStates.PlayerInfo)
        {
            return;
        }
        if (InputHandler.GetAction(ws.gm.gc.Player.Shoot))
        {
            if (isPressInput)
            {
                if (isInputReleased)
                {
                    isInputReleased = false;
                    if (fireRateCounter >= fireRate)
                    {
                        fireRateCounter = 0;
                        if (ws.gm.CheckAmmo(ammoCost, ammoType) >= 0)
                        {
                            ws.StartCoroutine(Shoot(ws));
                        }
                    }
                }
            }
            else
            {
                if (fireRateCounter >= fireRate && ws.IsActive)
                {
                    fireRateCounter = 0;
                    if (ws.gm.CheckAmmo(ammoCost, ammoType) >= 0)
                    {
                        ws.StartCoroutine(Shoot(ws));
                    }
                }
            }
        }
        else
        {
            if (!isInputReleased)
            {
                isInputReleased = true;
            }
        }
    }
    public virtual void CastWeaponLaser(LineRenderer laser)
    {
        Vector3 startPos = laser.gameObject.transform.position + laser.gameObject.transform.up * laserProfile.StartDistance;
        Vector3 endPos = startPos + laser.transform.up * laserProfile.LaserLength;

        RaycastHit2D hit = Physics2D.Raycast(laser.transform.position, laser.transform.up, laserProfile.LaserLength, LayerMask.GetMask("Walls"));
        if (hit)
        {
            endPos = hit.point;
        }
        Vector3 midPos = startPos + laser.transform.up * (Vector3.Distance(startPos, endPos) / 2);
        laser.SetPosition(0, startPos);
        laser.SetPosition(1, midPos);
        laser.SetPosition(2, endPos);
    }
    public abstract IEnumerator Shoot(WeaponStats ws);
}
