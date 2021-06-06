using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanguardAI : AIController
{
    [SerializeField]
    EntityStats Shield;
    [SerializeField]
    float MeleeRange;
    float LerpSpeed = 0.3f, SlowdownLerpSpeed = 0.3f;
    float MoveSpeed = 0;
    [HideInInspector]
    public Vector2 CommandedPoint;
    [HideInInspector]
    public bool IsCommanded;
    private void Update()
    {
        if (IsDeathDone)
        {
            return;
        }
        if (!IsDeathDone)
        {
            if (es.IsDead)
            {
                RunAIDeath();
                return;
            }
        }
        if (es.IsKnockback||es.IsStunned)
        {
            SendAnimatorBool("IsWalk", false);
            SendAnimatorBool("IsMelee", false);
            SendAnimatorBool("IsShoot", false);
            IsExecuting = false;
            StopAllCoroutines();
            return;
        }
        if (Shield.gameObject.activeInHierarchy == true)
        {
            if (WasShieldHit())
            {
                SendAnimatorTrigger("IsBlock");
            }
        }
        WasShieldHit();
        if (CurrentBehaviorState == AIBehavirorStates.State1)
        {
            if (CanSeePlayer())
            {
                es.ac.PlaySound("VanguardAlert", AudioMaster.SoundTypes.SFX, true, false);
                SendAnimatorBool("IsShieldIdle", true);
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State2;
                return;
            }
        }
        else if (CurrentBehaviorState == AIBehavirorStates.State2)
        {
            //Send to state 3 if this eneity is commanded
            if (IsCommanded)
            {
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State3;
                return;
            }
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior2());
            }
        }
        else if(CurrentBehaviorState == AIBehavirorStates.State3)
        {
            //Send to state 2 if this entity is not commanded
            if (!IsCommanded)
            {
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State2;
                return;
            }
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior3());
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
        if (es.IsStunned)
        {
            IsExecuting = false;
            yield break;
        }
        //If AI can't see player and they arent in range
        if (!IsPlayerInRange() || !CanSeePlayer())
        {
            if (!isStationary)
            {
                SendAnimatorBool("IsWalk", true);
                Agent.isStopped = false;
                WalkToPoint(es.gm.Player.transform.position, es.EntityWalkSpeed);
            }
        }
        else if (Vector2.Distance(es.gm.Player.transform.position, transform.position) > MeleeRange)
        {
            if (!isStationary)
            {
                SendAnimatorBool("IsWalk", true);
                Agent.isStopped = false;
                WalkToPoint(es.gm.Player.transform.position, es.EntityWalkSpeed);
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
            }
        }
        else
        {
            Agent.isStopped = true;
            Agent.velocity = Vector2.zero;
            SendAnimatorBool("IsWalk", false);
            yield return StartCoroutine(Melee());
            yield return new WaitForSeconds(1);
        }
        IsExecuting = false;
        yield return null;
    }
    IEnumerator Behavior3()
    {
        WalkToCommandedPoint();
        for (float i = 0; i < 2.5f;)
        {
            RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
            WalkToCommandedPoint();
            i += 1 * Time.deltaTime;
            yield return null;
        }
        if (Vector2.Distance(es.gm.Player.transform.position, transform.position) < AttackRange && CanSeePlayer())
        {
            float r = Random.Range(0, 100);
            if (r > 60)
            {
                for (float i = 0; i < 1f;)
                {
                    LeadTarget(es.gm.Player.gameObject, 5, RotateSpeed * 4);
                    i += 1 * Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                for (float i = 0; i < 1f;)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    i += 1 * Time.deltaTime;
                    yield return null;
                }
            }
            StartCoroutine(Shoot(6));
            WalkToCommandedPoint();
        }
        IsExecuting = false;
        yield return null;
    }
    IEnumerator Melee()
    {
        es.ac.PlaySound("Swing1", AudioMaster.SoundTypes.SFX, true,false);
        SendAnimatorBool("IsMelee",true);
        Shield.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        EntityStats AlreadyHit = null;
        for(float i = 0; i < 0.1f;)
        {
            RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
            Vector2 HitboxSize = new Vector2(2, 4);
            Collider2D Hit = Physics2D.OverlapBox(AIFirePoint.transform.position,
                HitboxSize, transform.rotation.eulerAngles.x, LayerMask.GetMask("Player"));
            if (Hit != null&& AlreadyHit == null)
            {
                Hit.gameObject.GetComponent<EntityStats>().TakeDamage(AIDamage);
                AlreadyHit = Hit.gameObject.GetComponent<EntityStats>();
            }
            i += 1 * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.095f);
        SendAnimatorBool("IsMelee", false);
        Shield.gameObject.SetActive(true);
        yield return null;
    }
    IEnumerator Shoot(int Shots)
    {
        es.ac.PlaySound("ShotgunPrimary", AudioMaster.SoundTypes.SFX, true,false);
        SendAnimatorBool("IsShoot", true);
        for (int i = 0; i < Shots;i++)
        {
            float RadShotSpread = 20 * 0.0174533f;
            float SpreadX = Random.Range(AIFirePoint.transform.up.x - RadShotSpread, AIFirePoint.transform.up.x + RadShotSpread);
            float SpreadY = Random.Range(AIFirePoint.transform.up.y - RadShotSpread, AIFirePoint.transform.up.y + RadShotSpread);
            Vector2 ShotDir = new Vector2(SpreadX, SpreadY).normalized;

            GameObject Shot = Instantiate(AIProjectile,AIFirePoint.transform.position, AIFirePoint.transform.rotation);
            Shot.GetComponent<Projectile>().rb.velocity = ShotDir * Shot.GetComponent<Projectile>().ProjectileSpeed;
            Shot.GetComponent<Projectile>().ProjectileDamage = new Damage(50, false, false, false,false);
            Shot.GetComponent<Projectile>().HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(AIFirePoint.transform.position, ShotDir, 100f, LayerMask.GetMask("Walls"));
            Shot.GetComponent<Projectile>().WallPosition = Hit.point;
            Shot.GetComponent<Projectile>().ProjectileDamage.Direction = transform.up;
        }
        yield return new WaitForSeconds(0.35f);
        SendAnimatorBool("IsShoot", false);
    }
    float PreviousShieldHealth;
    bool WasShieldHit()
    {
        if(Shield.EntityHealth < PreviousShieldHealth)
        {
            PreviousShieldHealth = Shield.EntityHealth;
            return true;
        }
        PreviousShieldHealth = Shield.EntityHealth;
        return false;
    }
    void WalkToCommandedPoint()
    {
        if (isStationary)
        {
            return;
        }
        if ((Vector2)transform.position != CommandedPoint)
        {
            SendAnimatorBool("IsWalk", true);
            WalkToPoint(CommandedPoint,es.EntityWalkSpeed);
        }
        else
        {
            SendAnimatorBool("IsWalk", false);
        }
    }
}
