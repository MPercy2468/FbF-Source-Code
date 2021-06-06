using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EMagBolt")]
public class EMagBoltChip : WeaponChip
{
    public override IEnumerator Shoot(WeaponStats ws)
    {
        ws.CanBreakProjectiles = ws.gm.Player.overclock.isBulletBuster;
        ws.ac.PlaySound("CrossbowPrimary", AudioMaster.SoundTypes.SFX, true, false);
        ws.StartCoroutine(ws.gm.cc.CameraRecoil(ws.transform.up * -1, 0.3f));
        ws.gm.RemoveAmmo(ammoCost, ammoType);
        ws.IsBeingFired = true;
        ws.SendAnimatorBool("IsShoot", true);
        GameObject Shot = Instantiate(projectile, ws.FirePoint.position, ws.transform.parent.rotation);
        Projectile p = Shot.GetComponent<Projectile>();
        p.rb.velocity = ws.transform.up * p.ProjectileSpeed;
        p.ProjectileDamage = new Damage(damage);
        p.ProjectileDamage.Direction = ws.transform.up;
        RaycastHit2D Hit = Physics2D.Raycast(ws.transform.position, ws.transform.up, shootRaycastLength, LayerMask.GetMask("Walls"));
        p.WallPosition = Hit.point;
        p.CanBreakProjectiles = ws.CanBreakProjectiles;
        RaycastHit2D ReverseCheck = Physics2D.Raycast(ws.FirePoint.transform.position, ws.FirePoint.transform.up * -1, 1f, LayerMask.GetMask("Walls"));
        if (ReverseCheck || Vector2.Distance(Hit.point, ws.FirePoint.transform.position) < 0.6f)
        {
            p.DestroyProjectile();
        }
        yield return new WaitForSeconds(0.195f);
        ws.IsBeingFired = false;
        ws.SendAnimatorBool("IsShoot", false);
    }
}
