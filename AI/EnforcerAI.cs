using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnforcerAI : AIController
{
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
        if (es.IsKnockback)
        {
            SendAnimatorBool("IsShoot", false);
            SendAnimatorBool("IsWalk", false);
            IsExecuting = false;
            StopAllCoroutines();
            return;
        }
        if (CurrentBehaviorState == AIBehavirorStates.State1)
        {
            if (CanSeePlayer())
            {
                es.ac.PlaySound("EnforcerAlert", AudioMaster.SoundTypes.SFX, true, false);
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State2;
            }
        }
        else if(CurrentBehaviorState == AIBehavirorStates.State2)
        {
            //Send to state 3 if this entity is stunned
            if (es.IsStunned)
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
        else if (CurrentBehaviorState == AIBehavirorStates.State3)
        {
            //Send to state 2 if entity is not stunned anymore
            if (!es.IsStunned)
            {
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State2;
                return;
            }
        }
        else if(CurrentBehaviorState == AIBehavirorStates.Beam)
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
        if (Vector2.Distance(es.gm.Player.transform.position, transform.position) > AttackRange||!CanSeePlayer())
        {
            if (!isStationary)
            {
                SendAnimatorBool("IsWalk", true);
                Agent.isStopped = false;
                WalkToPoint(es.gm.Player.transform.position, es.EntityWalkSpeed);
            }
        }
        else
        {
            Agent.isStopped = true;
            SendAnimatorBool("IsWalk", false);
            SendAnimatorBool("IsShootIdle", true);
            if (!CanSeePlayer())
            {
                IsExecuting = false;
                yield break;
            }
            float r = Random.Range(0, 100);
            if (r > 60)
            {
                for (float i = 0; i < 1f;)
                {
                    DrawAttackLine(AttackRange);
                    LeadTarget(es.gm.Player.gameObject, 5, RotateSpeed * 4);
                    i += 1 * Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                for (float i = 0; i < 1f;)
                {
                    DrawAttackLine(AttackRange);
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    i += 1 * Time.deltaTime;
                    yield return null;
                }
            }
            if (!CanSeePlayer())
            {
                IsExecuting = false;
                yield break;
            }
            yield return StartCoroutine(Shoot(3));
            if (Vector2.Distance(transform.position, es.gm.Player.transform.position) > 10)
            {
                Agent.isStopped = false;
                Vector2 PlayerDir = (es.gm.Player.transform.position - transform.position).normalized;
                WalkToPoint((Vector2)transform.position + PlayerDir * 3,es.EntityWalkSpeed);
                yield return new WaitForSeconds(0.5f);
            }
        }
        IsExecuting = false;
        yield return null;
    }
    IEnumerator Shoot(int Shots)
    {
        EraseAttackLine();
        SendAnimatorBool("IsShoot", true);
        if (IsCommanded)
        {
            Shots += 3;
        }
        for (int i = 0; i < Shots;)
        {
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true,false);
            GameObject Shot = Instantiate(AIProjectile, AIFirePoint.transform.position,AIFirePoint.transform.rotation);
            Shot.GetComponent<Projectile>().rb.velocity = AIFirePoint.transform.up*Shot.GetComponent<Projectile>().ProjectileSpeed;
            Shot.GetComponent<Projectile>().ProjectileDamage = AIDamage;
            Shot.GetComponent<Projectile>().HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, AIFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            Shot.GetComponent<Projectile>().WallPosition = Hit.point;
            yield return new WaitForSeconds(0.200f);
            i++;
        }
        SendAnimatorBool("IsShoot", false);
        yield return new WaitForSeconds(1);
    }
}
