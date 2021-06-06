using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressAI : AIController
{
    [SerializeField]
    EntityStats leftShield, rightShield, weakpoint;
    bool isLeftShieldDead, isRightShieldDead, isMissile;
    Vector2 launchDir = Vector2.zero;
    [SerializeField]
    GameObject explosionPFX;

    private void Update()
    {
        if (IsDeathDone)
        {
            return;
        }
        if (!IsDeathDone)
        {
            if (!isMissile)
            {
                if (es.IsDead)
                {
                    RunAIDeath();
                    return;
                }
            }
        }
        //Run missile mode for AI
        RunMissile();
        if (isMissile)
        {
            return;
        }
        //Check shield and weakpoint status
        CheckShieldStatus();
        CheckWeakpointStatus();
        if (es.IsKnockback || es.IsStunned)
        {
            IsExecuting = false;
            StopAllCoroutines();
            return;
        }
        if (CurrentBehaviorState == AIBehavirorStates.State1)
        {
            if (CanSeePlayer())
            {
                //es.ac.PlaySound("SniperAlert", AudioMaster.SoundTypes.SFX, true, false);
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State2;
            }
        }
        else if (CurrentBehaviorState == AIBehavirorStates.State2)
        {
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior2());
            }
        }
        else if (CurrentBehaviorState == AIBehavirorStates.Beam)
        {
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(BeamToPlayer());
            }
        }
    }
    IEnumerator Behavior1()
    {
        yield return null;
    }
    IEnumerator Behavior2()
    {
        if (Vector2.Distance(es.gm.Player.transform.position, transform.position) > AttackRange || !CanSeePlayer())
        {
            if (!isStationary)
            {
                EraseAttackLine();
                Agent.isStopped = false;
                WalkToPoint(es.gm.Player.transform.position, es.EntityWalkSpeed);
            }
        }
        else
        {
            Agent.isStopped = true;
            //Attempt to turn to face player
            for (float i = 0; i < 2; i += Time.deltaTime)
            {
                //Stop execution if missile is true
                if (isMissile)
                {
                    IsExecuting = false;
                    yield break;
                }
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                FindClosestEnemyAndDefend();
                yield return null;
            }
            if (!CanSeePlayer())
            {
                IsExecuting = false;
                yield break;
            }
        }
        IsExecuting = false;
        yield return null;
    }

    void CheckShieldStatus()
    {
        //Check if left and right shield are dead, send animation accordingly
        if (leftShield.IsDead&&!isLeftShieldDead)
        {
            leftShield.GetComponent<Animator>().SetTrigger("IsDead");
            rightShield.GetComponent<Animator>().SetTrigger("IsShake");
            SendAnimatorTrigger("IsShakeRight");
            isLeftShieldDead = true;
        }
        if (rightShield.IsDead&&!isRightShieldDead)
        {
            rightShield.GetComponent<Animator>().SetTrigger("IsDead");
            leftShield.GetComponent<Animator>().SetTrigger("IsShake");
            SendAnimatorTrigger("IsShakeLeft");
            isRightShieldDead = true;
        }
        //if both shields are dead, become a missile
        if (rightShield.IsDead && leftShield.IsDead&&!isMissile)
        {
            isMissile = true;
            launchDir = weakpoint.transform.up;
            es.ac.PlaySound("RocketFly", AudioMaster.SoundTypes.SFX, false, true);
            SendAnimatorTrigger("IsRocket");
        }
    }
    void CheckWeakpointStatus()
    {
        //If weakpoint is dead, become a missile
        if (weakpoint.IsDead && !isMissile)
        {
            isMissile = true;
            launchDir = weakpoint.transform.up;
            es.ac.PlaySound("RocketFly", AudioMaster.SoundTypes.SFX, false, true);
            SendAnimatorTrigger("IsRocket");
        }
    }

    void RunMissile()
    {
        if (!isMissile)
        {
            return;
        }
        Agent.isStopped = true;
        //Set velocity to 10 units per second forward
        es.rb.velocity = launchDir * 10;
        //Check for collisions
        Collider2D[] colCheck = Physics2D.OverlapCircleAll(transform.position, 1.25f, LayerMask.GetMask("Enemies") + LayerMask.GetMask("Player") + LayerMask.GetMask("Walls"));
        //Check if there are any valid collision points
        bool wasCollision = false;
        for(int i = 0; i < colCheck.Length; i++)
        {
            if (!es.CheckMatchingCollider(colCheck[i]))
            {
                wasCollision = true;
            }
        }
        //if collision detected, explode
        if (wasCollision)
        {
            
            es.rb.velocity = Vector2.zero;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2, LayerMask.GetMask("Enemies") + LayerMask.GetMask("Player"));
            foreach(Collider2D h in hits)
            {
                if (h.GetComponent<EntityStats>()&&!es.CheckMatchingCollider(h))
                {
                    Damage d = new Damage(AIDamage);
                    Vector2 damageDir = h.transform.position - transform.position;
                    damageDir = damageDir.normalized;
                    h.GetComponent<EntityStats>().TakeDamage(d);
                }
            }
            RunMissileDeath();
        }
    }
    void RunMissileDeath()
    {
        rightShield.GetComponent<Animator>().SetTrigger("IsDead");
        leftShield.GetComponent<Animator>().SetTrigger("IsDead");
        es.SetEnabledEntitySpriteRenderers(false);
        Instantiate(explosionPFX, transform.position, Quaternion.identity).SetActive(true);
        es.ac.PlaySound("HEExplosion", AudioMaster.SoundTypes.SFX, true, false);
        es.ac.StopSound("RocketFly");
        IsDeathDone = true;
        es.DestroyColliders();
    }

    void FindClosestEnemyAndDefend()
    {
        if (isMissile)
        {
            return;
        }
        Debug.Log("Find closest enemy");
    	Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, 15, LayerMask.GetMask("Enemies"));
    	Collider2D nearestEnemy = null;
    	float minDistance = Mathf.Infinity;
        Vector2 defensePosition = transform.position;
        for (int i = 0; i <  enemiesInRange.Length; i++){
            float distanceFromCenter = Vector2.Distance(transform.position, enemiesInRange[i].transform.position);
            if (!es.CheckMatchingCollider(enemiesInRange[i]) && distanceFromCenter < minDistance)
            {
    			minDistance = distanceFromCenter;
    			nearestEnemy = enemiesInRange[i];
                Vector2 defenseDir = es.gm.Player.transform.position - nearestEnemy.transform.position;
                defenseDir = defenseDir.normalized;
                if (!Physics2D.Raycast(transform.position, defenseDir, 3,LayerMask.GetMask("Walls")))
                {
                    Debug.Log("Defense Direction: " + defenseDir);
                    defensePosition = (Vector2)nearestEnemy.transform.position + defenseDir * 3;
                }
            }
    	}
        if ((Vector2)transform.position != defensePosition)
        {
            Agent.isStopped = false;
            WalkToPoint((Vector2)defensePosition, es.EntityWalkSpeed);
        }
    }
    public override void RunAIDeath()
    {
        StopAllCoroutines();
        EraseAttackLine();
        Agent.isStopped = true;
        Agent.velocity = Vector2.zero;
        Destroy(Agent);
        es.DestroyEntityShadows();
        es.rb.velocity = Vector2.zero;
        AIGFXRenderer.sortingLayerName = "GroundEffects";
        SendAnimatorTrigger("IsDeath");
        rightShield.GetComponent<Animator>().SetTrigger("IsDead");
        leftShield.GetComponent<Animator>().SetTrigger("IsDead");
        Instantiate(explosionPFX, transform.position, Quaternion.identity).SetActive(true);
        es.ac.PlaySound("HEExplosion", AudioMaster.SoundTypes.SFX, true, false);
        es.ac.StopSound("RocketFly");
        gameObject.AddComponent<GibGFX>().renderers = es.EntitySpriteRenderers;
        es.gm.GetComponent<LevelData>().scoreData.killCount++;
        IsDeathDone = true;
    }
}
