using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpreadShot")]
public class SpreadShotChip : WeaponChip
{
    public override IEnumerator Shoot(WeaponStats ws)
    {
        ws.CanBreakProjectiles = ws.gm.Player.overclock.isBulletBuster;
        ws.gm.RemoveAmmo(ammoCost, ammoType);
        ws.ac.PlaySound("ShotgunPrimary", AudioMaster.SoundTypes.SFX, true, false);
        ws.StartCoroutine(ws.gm.cc.CameraRecoil(ws.transform.up * -1, 0.3f));
        ws.IsBeingFired = true;
        ws.SendAnimatorBool("IsShoot", true);
        for (int Shots = 0; Shots < 5; Shots++)
        {
            float RadShotSpread = 10 * 0.0174533f;
            float SpreadX = Random.Range(ws.transform.up.x - RadShotSpread, ws.transform.up.x + RadShotSpread);
            float SpreadY = Random.Range(ws.transform.up.y - RadShotSpread, ws.transform.up.y + RadShotSpread);
            Vector2 ShotDir = new Vector2(SpreadX, SpreadY).normalized;

            GameObject Shot = Instantiate(projectile, ws.FirePoint.position, ws.transform.parent.rotation);
            Shot.GetComponent<Projectile>().rb.velocity = ShotDir * Shot.GetComponent<Projectile>().ProjectileSpeed;
            Shot.GetComponent<Projectile>().ProjectileDamage = new Damage(damage);
            RaycastHit2D Hit = Physics2D.Raycast(ws.FirePoint.transform.position, ShotDir, shootRaycastLength, LayerMask.GetMask("Walls"));
            Shot.GetComponent<Projectile>().WallPosition = Hit.point;
            Shot.GetComponent<Projectile>().ProjectileDamage.Direction = ws.transform.up;
            RaycastHit2D ReverseCheck = Physics2D.Raycast(ws.FirePoint.transform.position, ws.FirePoint.transform.up * -1, 1f, LayerMask.GetMask("Walls"));
            if (ReverseCheck || Vector2.Distance(Hit.point, ws.FirePoint.transform.position) < 0.6f)
            {
                Shot.GetComponent<Projectile>().DestroyProjectile();
            }
        }
        yield return new WaitForSeconds(0.195f);
        ws.IsBeingFired = false;
        ws.SendAnimatorBool("IsShoot", false);
    }
}
