using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaOrbProjectile : Projectile
{
    [SerializeField]
    EntityStats es;
    float damageThreshold = 150,damageAmountTaken;
    float aoeDamageRadius = 10;
    float aoeDamageCounter = 1, aoeDamageCooldown = 1;
    float previousHealth;
    int level = 1;
    [SerializeField]
    List<LineRenderer> aoeDamageLines = new List<LineRenderer>();
    float damageLinesAlpha = 0;
    private void Start()
    {
        ac.PlaySound(AmbientSound, AudioMaster.SoundTypes.SFX, false, true);
        previousHealth = es.EntityHealth;
    }
    public override void ProjectileLogic()
    {
        ReduceDamageLinesAlpha();
        if (!IsDestroyed)
        {
            if (Vector2.Distance(OriginPosition, WallPosition) <= Vector2.Distance(transform.position, OriginPosition))
            {
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
            if(aoeDamageCounter >= aoeDamageCooldown)
            {
                aoeDamageCounter = 0;
                DoAoeDamage();
            }
            else
            {
                aoeDamageCounter += Time.deltaTime;
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
        //Deal explosive damage
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
        //Destroy projectile
        base.DestroyProjectile();
    }

    bool IsHealthLower()
    {
        if(previousHealth > es.EntityHealth)
        {
            previousHealth = es.EntityHealth;
            return true;
        }
        return false;
    }
    public void AugmentDamage(Damage damage)
    {
        for(int i = 0; i < damage.StatusEffects.Count; i++)
        {
            ProjectileDamage.StatusEffects.Add(damage.StatusEffects[i]);
        }
        damageAmountTaken += damage.DamageAmount;
        //Check damage threshold and increase size of projectile
        if(damageAmountTaken >= damageThreshold)
        {
            //change GFX
            ProjectileAnimator.SetTrigger("IsNextStage");
            //increase threshold
            damageThreshold += damageAmountTaken;
            //change size
            switch (level)
            {
                case 1:
                    gameObject.GetComponent<CircleCollider2D>().radius = 0.5f;
                    es.gameObject.GetComponent<CircleCollider2D>().radius = 0.5F;
                    break;
                case 2:
                    gameObject.GetComponent<CircleCollider2D>().radius = 0.6f;
                    es.gameObject.GetComponent<CircleCollider2D>().radius = 0.6F;
                    break;
                case 3:
                    gameObject.GetComponent<CircleCollider2D>().radius = 0.9f;
                    es.gameObject.GetComponent<CircleCollider2D>().radius = 0.9F;
                    break;
            }
            //increase damage
            ProjectileDamage.DamageAmount += 50;
            //increase AOE radius
            aoeDamageRadius += 5;
        }
    }
    void DoAoeDamage()
    {
        //Make new instance of damage for calculation
        Damage d = new Damage(ProjectileDamage);
        d.DamageAmount *= 0.5f;
        //Array for referencing hit enemies
        Collider2D[] enemiesToHit = Physics2D.OverlapCircleAll(transform.position, aoeDamageRadius, LayerMask.GetMask("Enemies"));
        //Array for enemies already hit
        List<EntityStats> alreadyHits = new List<EntityStats>();
        //Add damage lines if there are more enemies hit than there are renderers
        if(aoeDamageLines.Count < enemiesToHit.Length)
        {
            int diff = enemiesToHit.Length - aoeDamageLines.Count;
            for(int i = 0; i < diff; i++)
            {
                aoeDamageLines.Add(Instantiate(aoeDamageLines[0].gameObject, transform, false).GetComponent<LineRenderer>());
            }
        }
        //Damage each valid enemy hit
        for(int i = 0; i < enemiesToHit.Length; i++)
        {
            //Damage enemy and draw line if raycast is possible
            //Do not damage enemies already hit
            Vector2 dir = enemiesToHit[i].transform.position - transform.position;
            dir = dir.normalized;
            if (!Physics2D.Raycast(transform.position, dir, Vector2.Distance(transform.position, enemiesToHit[i].transform.position), LayerMask.GetMask("Walls")))
            {
                if (!alreadyHits.Contains(enemiesToHit[i].gameObject.GetComponentInParent<EntityStats>()))
                {
                    if (!enemiesToHit[i].gameObject.GetComponentInParent<EntityStats>().IsWeakpoint)
                    {
                        DrawLineToTarget(transform.position, enemiesToHit[i].transform.position, aoeDamageLines[i]);
                        enemiesToHit[i].gameObject.GetComponentInParent<EntityStats>().TakeDamage(d);
                        alreadyHits.Add(enemiesToHit[i].gameObject.GetComponentInParent<EntityStats>());
                    }
                }
            }
        }
    }

    void DrawLineToTarget(Vector2 origin,Vector2 target,LineRenderer line)
    {
        //Get number of kinks for line given distance
        int renderDistance = Mathf.RoundToInt(Vector2.Distance(origin, target));
        Vector3[] points = new Vector3[renderDistance];
        //Get Direction to target
        Vector2 dir = target - origin;
        dir = dir.normalized;
        Vector2 sideDir = new Vector2(dir.y, dir.x);
        //Assign origin and end positions
        points[0] = origin;
        points[points.Length - 1] = target;
        //Calculate points for line kinks
        for(int i = 1; i < points.Length-1; i++)
        {
            points[i] = origin + dir * i;
            points[i] += (Vector3)sideDir * Random.Range(-1,1f);
        }
        //Assign points to line renderer
        line.positionCount = points.Length;
        line.SetPositions(points);
        //Set line to be visible
        Color c = line.startColor;
        c.a = 255;
        line.startColor = c;
        line.endColor = c;
    }

    void ReduceDamageLinesAlpha()
    {
        for(int i = 0; i < aoeDamageLines.Count; i++)
        {
            Color c = aoeDamageLines[i].startColor;
            c.a -= 10 * Time.deltaTime;
            c.a = Mathf.Clamp(c.a, 0, 255);
            aoeDamageLines[i].startColor = c;
            aoeDamageLines[i].endColor = c;
        }
    }
}
