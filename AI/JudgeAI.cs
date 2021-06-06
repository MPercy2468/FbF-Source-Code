using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeAI : AIController
{
    [SerializeField]
    float ShootRotateSpeed;
    [SerializeField]
    GameObject commandedMarker;
    bool IsCommanding;
    VanguardAI Vanguard;
    List<EnforcerAI> Enforcers = new List<EnforcerAI>();
    List<EnforcerAI> EnforcersToRemove = new List<EnforcerAI>();
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
                if (Vanguard != null)
                {
                    Vanguard.IsCommanded = false;
                }
                foreach (EnforcerAI e in Enforcers)
                {
                    e.IsCommanded = false;
                }
                RunAIDeath();
                return;
            }
        }
        if (es.IsKnockback||es.IsStunned)
        {
            EraseAttackLine();
            SendAnimatorBool("IsWalk", false);
            SendAnimatorBool("IsShoot", false);
            IsExecuting = false;
            StopAllCoroutines();
            return;
        }
        if (CurrentBehaviorState == AIBehavirorStates.State1)
        {
            if (CanSeePlayer())
            {
                es.ac.PlaySound("JudgeAlert", AudioMaster.SoundTypes.SFX, true, false);
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State2;
                return;
            }
        }
        else if (CurrentBehaviorState == AIBehavirorStates.State2)
        {
            //Send to state 4 if entity can command units
            if (IsCommanding)
            {
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State4;
            }
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior2());
            }
        }
        else if (CurrentBehaviorState == AIBehavirorStates.State4)
        {
            if (!IsCommanding)
            {
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State2;
                return;
            }
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior4());
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
        GetMinions();
        CheckMinions();
        if (!IsPlayerInRange() || !CanSeePlayer())
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
            SendAnimatorTrigger("IsDraw");
            if (!CanSeePlayer())
            {
                IsExecuting = false;
                yield break;
            }
            float r = Random.Range(0, 100);
            if (r > 60)
            {
                for (float i = 0; i < 1.5f;)
                {
                    DrawAttackLine(AttackRange);
                    GetVanguardPosition();
                    LeadTarget(es.gm.Player.gameObject, 5, RotateSpeed * 4);
                    i += 1 * Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                for (float i = 0; i < 1.5f;)
                {
                    DrawAttackLine(AttackRange);
                    GetVanguardPosition();
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
            yield return StartCoroutine(Shoot(6));
        }
        IsExecuting = false;
        yield return null;
    }

    IEnumerator Behavior3()
    {

        IsExecuting = false;
        yield return null;
    }
    IEnumerator Behavior4()
    {
        Agent.isStopped = true;
        GetMinions();
        CheckMinions();
        GetVanguardPosition();

        SendAnimatorBool("IsWalk", false);
        SendAnimatorTrigger("IsDraw");
        if (!CanSeePlayer())
        {
            IsExecuting = false;
            yield break;
        }
        float r = Random.Range(0, 100);
        if(r > 60)
        {
            for (float i = 0; i < 1.5f;)
            {
                DrawAttackLine(AttackRange);
                GetVanguardPosition();
                LeadTarget(es.gm.Player.gameObject, 5, RotateSpeed * 4);
                i += 1 * Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            for (float i = 0; i < 1.5f;)
            {
                DrawAttackLine(AttackRange);
                GetVanguardPosition();
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
        yield return StartCoroutine(Shoot(6));
        IsExecuting = false;
        yield return null;
    }

    IEnumerator Shoot(int Shots)
    {
        EraseAttackLine();
        SendAnimatorBool("IsShoot", true);
        for (int i = 0; i < Shots;)
        {
            es.ac.PlaySound("GunShot1", AudioMaster.SoundTypes.SFX, true,false);
            RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
            GameObject Shot = Instantiate(AIProjectile, AIFirePoint.transform.position, AIFirePoint.transform.rotation);
            Shot.GetComponent<Projectile>().rb.velocity = AIFirePoint.transform.up * Shot.GetComponent<Projectile>().ProjectileSpeed;
            Shot.GetComponent<Projectile>().ProjectileDamage = AIDamage;
            Shot.GetComponent<Projectile>().HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, AIFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            Shot.GetComponent<Projectile>().WallPosition = Hit.point;
            yield return new WaitForSeconds(0.100f);
            i++;
        }
        SendAnimatorBool("IsShoot", false);
        yield return new WaitForSeconds(1);
    }
    void GetVanguardPosition()
    {
        Vector2 VanguardPos = Vector2.zero;
        if(Vanguard != null)
        {
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, (es.gm.Player.transform.position - transform.position).normalized, 2.5f, LayerMask.GetMask("Walls"));
            if (!Hit)
            {
                VanguardPos = transform.position + (es.gm.Player.transform.position - transform.position).normalized * 2.5f;
                Vanguard.IsCommanded = true;
                Vanguard.CommandedPoint = VanguardPos;
            }
            else
            {
                Vanguard.IsCommanded = false;
            }
        }
    }
    void CheckMinions()
    {
        if (Vanguard != null)
        {
            IsCommanding = true;
            GetVanguardPosition();
            Vanguard.IsCommanded = true;
            if (Vanguard.es.IsDead)
            {
                Vanguard = null;
            }
        }
        if (Vanguard == null)
        {
            IsCommanding = false;
        }
        foreach(EnforcerAI e in Enforcers)
        {
            if (Vector2.Distance(transform.position, e.transform.position) > 15||e.es.IsDead)
            {
                e.IsCommanded = false;
                EnforcersToRemove.Add(e);
            }
        }
        foreach(EnforcerAI e in EnforcersToRemove)
        {
            Enforcers.Remove(e);
        }
        EnforcersToRemove.Clear();
    }
    void GetMinions()
    {
        Collider2D[] Minions = Physics2D.OverlapCircleAll(transform.position, 15, LayerMask.GetMask("Enemies"));
        for(int i = 0; i < Minions.Length; i++)
        {
            if (Minions[i].gameObject.GetComponentInParent<VanguardAI>()!=null)
            {
                if(Minions[i].gameObject.GetComponentInParent<VanguardAI>().IsCommanded == false)
                {
                    if (Vanguard == null)
                    {
                        Vanguard = Minions[i].gameObject.GetComponentInParent<VanguardAI>();
                        GameObject m = Instantiate(commandedMarker, Vanguard.transform);
                        m.GetComponent<Marker>().Target = Minions[i].GetComponentInParent<EntityStats>();
                        m.GetComponent<CommandedMarker>().v = Vanguard;
                    }
                }
            }
            else if(Minions[i].gameObject.GetComponentInParent<EnforcerAI>()!= null)
            {
                if(Minions[i].gameObject.GetComponentInParent<EnforcerAI>().IsCommanded == false)
                {
                    Minions[i].gameObject.GetComponentInParent<EnforcerAI>().IsCommanded = true;
                    GameObject m = Instantiate(commandedMarker, Minions[i].transform);
                    m.GetComponent<Marker>().Target = Minions[i].GetComponentInParent<EntityStats>();
                    m.GetComponent<CommandedMarker>().e = Minions[i].GetComponentInParent<EnforcerAI>();
                    Enforcers.Add(Minions[i].gameObject.GetComponentInParent<EnforcerAI>());
                }
            }
        }

    }
}
