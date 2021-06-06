using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlakProjectile : Projectile
{
    //Gameobject for spawning flak projectiles on explosion
    [SerializeField]
    GameObject flakProjectile;
    [SerializeField]
    Damage flakProjectileDamage;
    //bool to allow projectile to explode near valid target
    [SerializeField]
    bool isProximityExplode;
    //float value for proximity to target before explode
    [SerializeField]
    float explodeProximity;
    public override void ProjectileCollisionLogic(Collider2D collision)
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
                    if (IsExplosive)
                    {
                        collision.transform.gameObject.GetComponentInParent<EntityStats>().TakeDamage(ProjectileDamage);
                        Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, ProjectileExplosionRadius,
                            LayerMask.GetMask("Enemies") + LayerMask.GetMask("Player"));
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
                        if (!CanPierceTargets)
                        {
                            ac.StopSound(AmbientSound);
                            DestroyProjectile();
                        }
                    }
                    else
                    {
                        collision.transform.gameObject.GetComponentInParent<EntityStats>().TakeDamage(ProjectileDamage);
                        if (!CanPierceTargets)
                        {
                            ac.StopSound(AmbientSound);
                            DestroyProjectile();
                        }
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
            if (isProximityExplode)
            {
                CheckExplodeProximity();
            }
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

    void SpawnFlakpProjectiles()
    {
        Vector2 projectileDir = new Vector2(1, 0);
        Vector2 dirToAdd = new Vector2(-0.86f, 0.5f);
        for (int i = 0; i < 13;i++)
        {
            //get angle for firing;
            float angle = i * Mathf.PI * 2f / 13;
            Vector2 angleDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            //Spawn flak projectile
            GameObject shot = Instantiate(flakProjectile,(Vector2)transform.position+angleDir, transform.rotation);
            Projectile p = shot.GetComponent<Projectile>();
            p.rb.velocity = angleDir * 15;
            p.ProjectileDamage = flakProjectileDamage;
            p.HitPlayer = HitPlayer;
            RaycastHit2D Hit = Physics2D.Raycast((Vector2)transform.position+angleDir, angleDir, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //angle projectile to face direction
            float projAngle = Mathf.Atan2(angleDir.y, angleDir.x);
            projAngle = Mathf.Rad2Deg * projAngle;
            projAngle -= 90;
            shot.transform.eulerAngles = new Vector3(0, 0, projAngle);
        }
    }
    void CheckExplodeProximity()
    {
        if (HitPlayer)
        {
            if (Vector2.Distance(transform.position, gm.Player.transform.position) < explodeProximity)
            {
                Vector2 dir = gm.Player.transform.position - transform.position;
                dir = dir.normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Vector2.Distance(transform.position, 
                    gm.Player.transform.position), LayerMask.GetMask("Walls"));
                if (!hit)
                {
                    DestroyProjectile();
                }
            }
        }
        else
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, explodeProximity, LayerMask.GetMask("Enemies"));
            if (col)
            {
                if (!col.GetComponentInParent<EntityStats>().IsPlayer)
                {
                    Vector2 dir = col.transform.position - transform.position;
                    dir = dir.normalized;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Vector2.Distance(transform.position,
                        col.transform.position), LayerMask.GetMask("Walls"));
                    if (!hit)
                    {
                        DestroyProjectile();
                    }
                }
            }
        }

    }
    public override void DestroyProjectile()
    {
        SpawnFlakpProjectiles();
        base.DestroyProjectile();
    }
}
