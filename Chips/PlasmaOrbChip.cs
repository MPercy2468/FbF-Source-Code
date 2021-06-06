using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlasmaOrb")]
public class PlasmaOrbChip : WeaponChip
{
    public override IEnumerator Shoot(WeaponStats ws)
    {
        ws.CanBreakProjectiles = ws.gm.Player.overclock.isBulletBuster;
        ws.ac.PlaySound("GreenGunPrimary", AudioMaster.SoundTypes.SFX, true, false);
        ws.StartCoroutine(ws.gm.cc.CameraRecoil(ws.transform.up * -1, 0.1f));
        ws.gm.RemoveAmmo(ammoCost, ammoType);
        ws.IsBeingFired = true;
        ws.SendAnimatorBool("IsShoot", true);
        GameObject Shot = Instantiate(projectile, ws.FirePoint.position, ws.transform.parent.rotation);
        Projectile p = Shot.GetComponent<Projectile>();
        p.rb.velocity = ws.transform.up * p.ProjectileSpeed;
        p.ProjectileDamage = new Damage(damage);
        RaycastHit2D Hit = Physics2D.Raycast(ws.FirePoint.transform.position, ws.FirePoint.transform.up, shootRaycastLength, LayerMask.GetMask("Walls"));
        p.WallPosition = Hit.point;
        p.ProjectileDamage.Direction = ws.transform.up;
        p.CanBreakProjectiles = ws.CanBreakProjectiles;
        yield return new WaitForSeconds(0.195f);
        ws.IsBeingFired = false;
        ws.SendAnimatorBool("IsShoot", false);
    }
}
