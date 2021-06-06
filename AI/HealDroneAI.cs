using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealDroneAI : AIController
{
    [SerializeField]
    GameObject HealMarker;
    List<EntityStats> HealTargets = new List<EntityStats>();
    List<GameObject> HealMarkers = new List<GameObject>();
    TechnomancerAI Technomancer;
    float HealCounter, HealCooldown=3;
    private void Start()
    {
        Collider2D[] Colliders = Physics2D.OverlapCircleAll(transform.position, 3, LayerMask.GetMask("Enemies"));
        foreach(Collider2D c in Colliders)
        {
            if(c.gameObject.GetComponentInParent<TechnomancerAI>() != null)
            {
                Technomancer = c.gameObject.GetComponentInParent<TechnomancerAI>();
                break;
            }
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
                es.ac.PlaySound("Droid1", AudioMaster.SoundTypes.SFX, true,false);
                Agent.isStopped = true;
                Agent.velocity = Vector2.zero;
                Destroy(Agent);
                SendAnimatorTrigger("IsDeath");
                int Length = HealMarkers.Count;
                for(int i = 0; i < Length; i++)
                {
                    Destroy(HealMarkers[i]);
                }
                foreach(EntityStats e in HealTargets)
                {
                    e.HasDrone = false;
                }
                StopAllCoroutines();
                es.DestroyEntityShadows();
                es.rb.velocity = Vector2.zero;
                IsDeathDone = true;
                es.gm.GetComponent<LevelData>().scoreData.killCount++;
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
        CheckForHealTargets();
        HealAllTargets();
        if (CurrentBehaviorState == AIBehavirorStates.State1)
        {
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior1());
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
        if(HealTargets.Count == 0)
        {
            WalkToPoint((Vector2)Technomancer.transform.position + Random.insideUnitCircle*5, es.EntityWalkSpeed);
        }
        else if(HealTargets.Count > 0)
        {
            WalkToPoint(GetAverageHealPosition(),es.EntityWalkSpeed);
        }
        yield return new WaitForSeconds(1);
        IsExecuting = false;
        yield return null;
    }
    Vector2 GetAverageHealPosition()
    {
        Vector2 Result = Vector2.zero;
        foreach(EntityStats e in HealTargets)
        {
            Result += (Vector2)e.transform.position;
        }
        Result /= HealTargets.Count;
        Result += Random.insideUnitCircle * 5;
        return Result;
    }
    void CheckForHealTargets()
    {
        if(HealTargets.Count < 4)
        {
            Collider2D[] Colliders = Physics2D.OverlapCircleAll(transform.position, 15, LayerMask.GetMask("Enemies"));
            foreach (Collider2D c in Colliders)
            {
                EntityStats e = c.gameObject.GetComponentInParent<EntityStats>();
                if (!e.IsShield&&!e.IsDrone&&!e.HasDrone&&!e.IsWeakpoint)
                {
                    e.HasDrone = true;
                    HealTargets.Add(e);
                    GameObject m = Instantiate(HealMarker, e.transform.position, Quaternion.identity);
                    m.GetComponent<Marker>().Target = e;
                    HealMarkers.Add(m);
                }
            }
        }
    }
    void HealAllTargets()
    {
        if(HealCounter >= HealCooldown)
        {
            HealCounter = 0;
            foreach(EntityStats e in HealTargets)
            {
                e.AddStatusEffect("Heal", HealCooldown,15,EntityStats.EntityTypes.None
                    ,GameMaster.AmmoTypes.None,Vector2.zero);
            }
        }
        HealCounter += Time.deltaTime;
    }
}
