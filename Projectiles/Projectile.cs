using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator ProjectileAnimator;
    [SerializeField]
    protected AudioController ac;
    [SerializeField]
    protected string AmbientSound, ImpactSound;
    public float ProjectileLifeTime;
    protected float LifeTimeCounter = 0;
    public bool CanBreakProjectiles,HitPlayer,IsExplosive;
    protected bool CanBeDestroyed = true;
    protected bool IsDestroyed;
    public Vector2 WallPosition,OriginPosition;
    public float ProjectileExplosionRadius;
    public float ProjectileSpeed = 50;
    [HideInInspector]
    public Damage ProjectileDamage;
    public int Ricochets;
    [HideInInspector]
    public Vector2 ReflectionNormal;
    public bool CanPierceTargets;
    protected GameMaster gm;
    [HideInInspector]
    public List<EntityStats> AlreadyHits = new List<EntityStats>();
    [SerializeField]
    protected GameObject destructionPFX;
    [SerializeField]
    protected Collider2D projectileCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProjectileCollisionLogic(collision);
    }

    private void Awake()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
        OriginPosition = transform.position;
    }
    private void Start()
    {
        ac.PlaySound(AmbientSound, AudioMaster.SoundTypes.SFX, true,true);
    }
    private void Update()
    {
        ProjectileLogic();
    }
    public virtual void LaunchProjectile(WeaponStats ws)
    {


    }
    public virtual void ProjectileLogic()
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
    public virtual void DestroyProjectile()
    {
        IsDestroyed = true;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        ProjectileAnimator.SetTrigger("IsHit");
        Destroy(gameObject.GetComponent<Collider2D>());
        if (destructionPFX != null)
        {
            GameObject d = Instantiate(destructionPFX, transform.position, Quaternion.identity);
            d.SetActive(true);
            d.GetComponentInChildren<AudioController>().PlaySound(ImpactSound, AudioMaster.SoundTypes.SFX, true, false);
        }
    }

    public virtual void ProjectileCollisionLogic(Collider2D collision)
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
                            LayerMask.GetMask("Enemies","Player","ProjectileHitboxes"));
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
    protected void Ricochet()
    {
        if (Ricochets > 0)
        {
            if (!HitPlayer)
            {
                gm.um.sm.IncreaseStyleScore(100, StyleMeter.styleScoreMultiplierEnum.Ricochet);
            }
            ac.PlaySound(ImpactSound, AudioMaster.SoundTypes.SFX, true,false);
            CanBeDestroyed = false;
            OriginPosition = transform.position;
            Vector2 RicochetDir = Vector2.Reflect(rb.velocity.normalized,ReflectionNormal);
            RaycastHit2D Hit = Physics2D.Raycast((Vector2)transform.position + RicochetDir * 1.3f, RicochetDir, 400f, LayerMask.GetMask("Walls"));
            float Rotate = Mathf.Atan2(RicochetDir.y, RicochetDir.x);
            Rotate = Mathf.Rad2Deg * Rotate;
            Rotate -= 90;
            transform.eulerAngles = new Vector3(0, 0, Rotate);
            WallPosition = Hit.point;
            ReflectionNormal = Hit.normal;
            rb.velocity = RicochetDir * ProjectileSpeed;
            Ricochets--;
        }
    }
}