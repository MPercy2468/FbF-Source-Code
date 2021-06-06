using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookProjectile : Projectile
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsDestroyed)
        {
            if (collision.transform.gameObject.GetComponentInParent<EntityStats>() != null)
            {
                if (HitPlayer && collision.transform.gameObject.GetComponentInParent<Player>() == null)
                {
                    return;
                }
                if (!HitPlayer && collision.transform.gameObject.GetComponentInParent<Player>() != null)
                {
                    return;
                }
                else
                {
                    ProjectileDamage.UpdateDirection(rb.velocity.normalized);
                    collision.transform.gameObject.GetComponentInParent<EntityStats>().TakeDamage(ProjectileDamage);
                    if (!CanPierceTargets)
                    {
                        ac.StopSound(AmbientSound);
                        WallPosition = collision.transform.position;
                        DestroyProjectile();
                    }
                }
            }
            else if (collision.transform.gameObject.GetComponent<Projectile>() != null)
            {
                if (CanBreakProjectiles)
                {
                    collision.transform.gameObject.GetComponent<Projectile>().DestroyProjectile();
                }
            }
        }
    }
    public override void ProjectileLogic()
    {
        if (!IsDestroyed)
        {
            if (Vector2.Distance(OriginPosition, WallPosition) <= Vector2.Distance(transform.position, OriginPosition))
            {
                ProjectileDamage.UpdateDirection(rb.velocity.normalized);
                if (IsExplosive)
                {
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
                }
                if (CanBeDestroyed)
                {
                    ac.PlaySound(ImpactSound, AudioMaster.SoundTypes.SFX, true, false);
                    ac.StopSound(AmbientSound);
                    DestroyProjectile();
                    return;
                }
            }
            else
            {
                CanBeDestroyed = true;
            }
            if (Vector2.Distance(OriginPosition, transform.position) >= Vector2.Distance(OriginPosition,OriginPosition + (Vector2)transform.up*ProjectileLifeTime))
            {
                StopProjectile();
            }
        }
    }
    public override void DestroyProjectile()
    {
        IsDestroyed = true;
        rb.velocity = Vector2.zero;
        transform.position = WallPosition;
        gm.Player.grappleHook.target = this.gameObject;
        gm.Player.grappleHook.PullPlayerToTarget();
    }
    void StopProjectile()
    {
        IsDestroyed = true;
        rb.velocity = Vector2.zero;
        gm.Player.grappleHook.StopPull();
    }
}
