using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoundAI : AIController
{
    bool isLunging;
    [SerializeField]
    float BiteRange;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLunging)
        {
            return;
        }
        if(collision.transform.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            EraseAttackLine();
            StopCoroutine(Lunge());
            SendAnimatorBool("IsLunge", false);
            SendAnimatorBool("IsLungeCharge", false);
            isLunging = false;
            es.rb.velocity = Vector2.zero;
            es.AddStatusEffect("Stun", 2, 0, EntityStats.EntityTypes.None, GameMaster.AmmoTypes.None, Vector2.zero);
        }
    }
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
            IsExecuting = false;
            StopAllCoroutines();
            return;
        }
        if (CurrentBehaviorState == AIBehavirorStates.State1)
        {
            if (CanSeePlayer())
            {
                es.ac.PlaySound("HoundAlert", AudioMaster.SoundTypes.SFX, true, false);
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
            SendAnimatorBool("IsWalk", true);
            Agent.isStopped = false;
            WalkToPoint(es.gm.Player.transform.position, es.EntityWalkSpeed);
        }
        else
        {
            SendAnimatorBool("IsWalk", false);
            Agent.isStopped = true;
            //Prepare to lunge
            for(float t = 0;t<2;t += Time.deltaTime)
            {
                DrawAttackLine(AttackRange);
                SendAnimatorBool("IsLungeCharge", true);
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < BiteRange)
                {
                    SendAnimatorBool("IsLungeCharge", false);
                    yield return StartCoroutine(Bite());
                    IsExecuting = false;
                    yield break;
                }
                yield return null;
            }
            //Lunge
            yield return StartCoroutine(Lunge());
            Collider2D[] allies = Physics2D.OverlapCircleAll(transform.position, 30, LayerMask.GetMask("Enemies"));
            Vector2 runPos = Vector2.zero;
            foreach(Collider2D c in allies)
            {
                if (c.GetComponentInParent<SniperAI>() != null)
                {
                    runPos = c.transform.position;
                    break;
                }
            }
            if (runPos != Vector2.zero)
            {
                WalkToPoint(runPos, es.EntityWalkSpeed);
                yield return new WaitForSeconds(2f);
            }
        }
        IsExecuting = false;
        yield return null;
    }
    IEnumerator Lunge()
    {
        EraseAttackLine();
        SendAnimatorBool("IsLunge", true);
        isLunging = true;
        Vector2 lungeDir = (es.gm.Player.transform.position - transform.position).normalized;
        es.rb.velocity = lungeDir * 60;
        bool isAlreadyHit = false;
        for(float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            if (!isAlreadyHit)
            {
                Collider2D p = Physics2D.OverlapCircle(transform.position, 1.2f, LayerMask.GetMask("Player"));
                if (p != null)
                {
                    p.GetComponent<EntityStats>().TakeDamage(AIDamage);
                    isAlreadyHit = true;
                }
            }
            yield return null;
        }
        SendAnimatorBool("IsLunge", false);
        SendAnimatorBool("IsLungeCharge", false);
        isLunging = false;
        es.rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1);
    }
    IEnumerator Bite()
    {
        for(float t = 0;t<0.1f;t += Time.deltaTime)
        {
            yield return null;
        }
    }
}
