using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileProjectile : Projectile
{
    public override void ProjectileLogic()
    {
        if (!IsDestroyed)
        {
            if (Vector2.Distance(OriginPosition, WallPosition) <= Vector2.Distance(transform.position, OriginPosition))
            {
                ProjectileDamage.UpdateDirection(rb.velocity.normalized);
                Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, ProjectileExplosionRadius,
                    LayerMask.GetMask("Enemies", "Player"));
                for (int x = 0; x < Hits.Length; x++)
                {
                    EntityStats Hit = Hits[x].gameObject.GetComponentInParent<EntityStats>();
                    if (Hit != null)
                    {
                        if (!AlreadyHits.Contains(Hit))
                        {
                            AlreadyHits.Add(Hit);
                            Hit.TakeDamage(ProjectileDamage);
                        }
                    }
                }
                AlreadyHits.Clear();
                Ricochet();
                if (CanBeDestroyed)
                {
                    ac.PlaySound(ImpactSound, AudioMaster.SoundTypes.SFX, true, false);
                    ac.StopSound(AmbientSound);
                    DestroyProjectile();
                }
            }
            else
            {
                CanBeDestroyed = true;
            }
            if (LifeTimeCounter >= ProjectileLifeTime)
            {
                DestroyProjectile();
            }
            LifeTimeCounter += 1 * Time.deltaTime;
        }
    }
    public override void DestroyProjectile()
    {
        IsDestroyed = true;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        ProjectileAnimator.SetTrigger("IsHit");
        Destroy(gameObject.GetComponent<CircleCollider2D>());
    }
}
