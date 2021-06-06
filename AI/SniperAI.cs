using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAI : AIController
{
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
                es.ac.PlaySound("SniperAlert", AudioMaster.SoundTypes.SFX, true, false);
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
        Vector2 dir = ((Vector2)es.gm.Player.transform.position - (Vector2)transform.position).normalized;
        if (Vector2.Distance(es.gm.Player.transform.position, transform.position) > AttackRange || !CanSeePlayer())
        {
            if (!isStationary)
            {
                EraseAttackLine();
                SendAnimatorBool("IsProne", false);
                SendAnimatorBool("IsWalk", true);
                Agent.isStopped = false;
                WalkToPoint(es.gm.Player.transform.position, es.EntityWalkSpeed);
            }
        }
        //Retreat if too close to player
        else if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < 5)
        {
            EraseAttackLine();
            SendAnimatorBool("IsProne", false);
            SendAnimatorBool("IsWalk", true);
            Agent.isStopped = false;
            RetreatFromPoint(es.gm.Player.gameObject, 20);
            yield return new WaitForSeconds(1);
        }
        else
        {
            Agent.isStopped = true;
            SendAnimatorBool("IsWalk", false);
            SendAnimatorBool("IsProne", true);
            if (!CanSeePlayerSightTime())
            {
                IsExecuting = false;
                yield break;
            }
            //Attempt to keep sights on player
            for (float i = 0; i < 2f;i+=Time.deltaTime)
            {
                if(Vector2.Distance(transform.position, es.gm.Player.transform.position) < 5)
                {
                    SendAnimatorBool("IsProne", false);
                    IsExecuting = false;
                    yield break;
                }
                DrawAttackLine(AttackRange);
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                yield return null;
            }
            if (!CanSeePlayer())
            {
                IsExecuting = false;
                yield break;
            }
            yield return StartCoroutine(Shoot(1));
        }
        IsExecuting = false;
        yield return null;
    }
    IEnumerator Shoot(int Shots)
    {
        EraseAttackLine();
        SendAnimatorBool("IsShoot", true);
        for (int i = 0; i < Shots;)
        {
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            GameObject Shot = Instantiate(AIProjectile, AIFirePoint.transform.position, AIFirePoint.transform.rotation);
            Shot.GetComponent<Projectile>().rb.velocity = AIFirePoint.transform.up * Shot.GetComponent<Projectile>().ProjectileSpeed;
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
