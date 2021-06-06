using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissileShot")]
public class MissileShotChip : WeaponChip
{
    public override IEnumerator Shoot(WeaponStats ws)
    {
        ws.CanBreakProjectiles = ws.gm.Player.overclock.isBulletBuster;
        ws.gm.RemoveAmmo(ammoCost, ammoType);
        ws.ac.PlaySound("RocketLaunch", AudioMaster.SoundTypes.SFX, true, false);
        ws.StartCoroutine(ws.gm.cc.CameraRecoil(ws.transform.up * -1, 1f));
        ws.IsBeingFired = true;
        ws.SendAnimatorBool("IsShoot", true);
        GameObject Shot = Instantiate(projectile, ws.FirePoint.position, ws.transform.parent.rotation);
        Shot.GetComponent<Projectile>().rb.velocity = ws.transform.up * Shot.GetComponent<Projectile>().ProjectileSpeed;
        Shot.GetComponent<Projectile>().IsExplosive = true;
        Shot.GetComponent<Projectile>().ProjectileExplosionRadius = 2;
        Shot.GetComponent<Projectile>().ProjectileDamage = new Damage(damage);
        RaycastHit2D Hit = Physics2D.Raycast(ws.transform.position, ws.transform.up, shootRaycastLength, LayerMask.GetMask("Walls"));
        Shot.GetComponent<Projectile>().WallPosition = Hit.point;
        Shot.GetComponent<Projectile>().ProjectileDamage.Direction = ws.transform.up;
        Shot.GetComponent<Projectile>().CanBreakProjectiles = ws.CanBreakProjectiles;
        RaycastHit2D ReverseCheck = Physics2D.Raycast(ws.FirePoint.transform.position, ws.FirePoint.transform.up * -1, 1f, LayerMask.GetMask("Walls"));
        if (ReverseCheck || Vector2.Distance(Hit.point, ws.FirePoint.transform.position) < 0.6f)
        {
            Shot.GetComponent<Projectile>().DestroyProjectile();
        }
        yield return new WaitForSeconds(0.195f);
        ws.IsBeingFired = false;
        ws.SendAnimatorBool("IsShoot", false);
    }
}
