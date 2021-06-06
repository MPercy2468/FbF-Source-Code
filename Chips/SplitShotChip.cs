using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitShot")]
public class SplitShotChip : WeaponChip
{
    float fireAngle = 30;
    bool isCharge = false;
    bool isSoundStarted = false;
    public override void ChipLogic(WeaponStats ws)
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
            Charge(ws);
        }
        else
        {
            if (isCharge)
            {
                isCharge = false;
                if (fireRateCounter >= fireRate && ws.IsActive)
                {
                    fireRateCounter = 0;
                    if (ws.gm.CheckAmmo(ammoCost, ammoType) >= 0)
                    {
                        ws.StartCoroutine(Shoot(ws));
                    }
                }
                fireAngle = 30;
                isSoundStarted = false;
                ws.ac.StopSound("CrossbowAmbient");
            }
        }
        ws.gameObject.GetComponent<DataTransfer>().transferFloats[0] = fireAngle;
    }
    void Charge(WeaponStats ws)
    {
        isCharge = true;
        if (fireAngle > 5)
        {
            if (!isSoundStarted)
            {
                isSoundStarted = true;
                ws.ac.PlaySound("CrossbowAmbient", AudioMaster.SoundTypes.SFX, false, true);
            }
            fireAngle -= 15 * Time.deltaTime;
        }
        else
        {
            isSoundStarted = false;
            fireAngle = 0;
        }
        ws.gameObject.GetComponent<DataTransfer>().transferFloats[0] = fireAngle;
    }
    public override IEnumerator Shoot(WeaponStats ws)
    {
        ws.CanBreakProjectiles = ws.gm.Player.overclock.isBulletBuster;
        ws.ac.PlaySound("CrossbowPrimary", AudioMaster.SoundTypes.SFX, true, false);
        ws.StartCoroutine(ws.gm.cc.CameraRecoil(ws.transform.up * -1, 0.3f));
        ws.gm.RemoveAmmo(ammoCost, ammoType);
        ws.IsBeingFired = true;
        ws.SendAnimatorBool("IsShoot", true);
        float ShotAngle = fireAngle;
        Quaternion OldFirePointRotation = ws.FirePoint.rotation;
        for (int i = 0; i < 2; i++)
        {
            Vector3 Rotation = Vector3.zero;
            if (i == 0)
            {
                Rotation = new Vector3(0, 0, ShotAngle);
            }
            else
            {
                Rotation = new Vector3(0, 0, ShotAngle * -1);
            }
            Rotation += ws.transform.parent.rotation.eulerAngles;
            Quaternion QuatRotation = Quaternion.Euler(Rotation);
            ws.FirePoint.transform.rotation = QuatRotation;
            GameObject AltShot = Instantiate(projectile, ws.FirePoint.position, QuatRotation);
            AltShot.GetComponent<Projectile>().rb.velocity = AltShot.transform.up * AltShot.GetComponent<Projectile>().ProjectileSpeed;
            AltShot.GetComponent<Projectile>().ProjectileDamage = new Damage(damage);
            AltShot.GetComponent<Projectile>().ProjectileDamage.Direction = ws.transform.up;
            RaycastHit2D AltHit = Physics2D.Raycast(ws.FirePoint.transform.position, ws.FirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            AltShot.GetComponent<Projectile>().WallPosition = AltHit.point;
            AltShot.GetComponent<Projectile>().ReflectionNormal = AltHit.normal;
            AltShot.GetComponent<Projectile>().Ricochets = 1;
            AltShot.GetComponent<Projectile>().CanBreakProjectiles = ws.CanBreakProjectiles;
            if (ShotAngle == 0)
            {
                AltShot.GetComponent<Projectile>().CanPierceTargets = true;
            }
            RaycastHit2D AltReverseCheck = Physics2D.Raycast(ws.FirePoint.transform.position, ws.FirePoint.transform.up * -1, 1f, LayerMask.GetMask("Walls"));
            if (AltReverseCheck || Vector2.Distance(AltHit.point, ws.FirePoint.transform.position) < 0.6f)
            {
                AltShot.GetComponent<Projectile>().DestroyProjectile();
            }
        }
        ws.FirePoint.transform.rotation = OldFirePointRotation;
        GameObject Shot = Instantiate(projectile, ws.FirePoint.position, ws.transform.parent.rotation);
        Shot.GetComponent<Projectile>().rb.velocity = ws.transform.up * Shot.GetComponent<Projectile>().ProjectileSpeed;
        Shot.GetComponent<Projectile>().ProjectileDamage = new Damage(damage);
        Shot.GetComponent<Projectile>().ProjectileDamage.Direction = ws.transform.up;
        RaycastHit2D Hit = Physics2D.Raycast(ws.transform.position, ws.transform.up, shootRaycastLength, LayerMask.GetMask("Walls"));
        Shot.GetComponent<Projectile>().WallPosition = Hit.point;
        Shot.GetComponent<Projectile>().ReflectionNormal = Hit.normal;
        Shot.GetComponent<Projectile>().Ricochets = 1;
        Shot.GetComponent<Projectile>().CanBreakProjectiles = ws.CanBreakProjectiles;
        if (ShotAngle == 0)
        {
            Shot.GetComponent<Projectile>().CanPierceTargets = true;
        }
        RaycastHit2D ReverseCheck = Physics2D.Raycast(ws.FirePoint.transform.position, ws.FirePoint.transform.up * -1, 1f, LayerMask.GetMask("Walls"));
        if (ReverseCheck || Vector2.Distance(Hit.point, ws.FirePoint.transform.position) < 0.6f)
        {
            Shot.GetComponent<Projectile>().DestroyProjectile();
        }
        yield return new WaitForSeconds(0.195f);
        ws.IsBeingFired = false;
        ws.SendAnimatorBool("IsShoot", false);
        fireAngle = 30;
    }
}
