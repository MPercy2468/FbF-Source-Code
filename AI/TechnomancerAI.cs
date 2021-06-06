using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnomancerAI : AIController
{
    Vector2 PreviousSafeSpot;
    [SerializeField]
    EntityStats Tank;
    [SerializeField]
    GameObject[] Drones;
    List<EntityStats> ActiveDrones = new List<EntityStats>();
    [SerializeField]
    int MaxDrones = 0;
    float DroneSpawnCounter, DroneSpawnCooldown=1.5f;
    [SerializeField]
    Damage ExplosionDamage;
    [SerializeField]
    GameObject ExplosionPFX,ExplosionLight;

    private void Start()
    {
        PreviousSafeSpot = transform.position;
        DroneSpawnCounter = DroneSpawnCooldown;
    }
    private void Update()
    {
        if (IsDeathDone)
        {
            return;
        }
        if (!IsDeathDone)
        {
            if (es.IsDead||Tank.IsDead)
            {
                Agent.isStopped = true;
                Agent.velocity = Vector2.zero;
                Destroy(Agent);
                SendAnimatorTrigger("IsDeath");
                Tank.DestroyColliders();
                Tank.DestroyEntityShadows();
                Tank.KillEntity();
                es.KillEntity();
                StopAllCoroutines();
                es.DestroyColliders();
                es.DestroyEntityShadows();
                es.rb.velocity = Vector2.zero;
                StartCoroutine(ExplosionLights());
                Invoke("Explode", 0.45f);
                foreach(EntityStats e in ActiveDrones)
                {
                    e.KillEntity();
                }
                IsDeathDone = true;
                es.gm.GetComponent<LevelData>().scoreData.killCount++;
                return;
            }
        }
        if (es.IsKnockback||es.IsStunned)
        {
            es.SetCollidersEnabled(true);
            IsExecuting = false;
            StopAllCoroutines();
            return;
        }
        ClearDrones();
        if (CurrentBehaviorState == AIBehavirorStates.State1)
        {
            if (CanSeePlayer())
            {
                es.ac.PlaySound("RobotOnline", AudioMaster.SoundTypes.SFX, true, false);
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
        IsExecuting = false;
        yield return null;
    }
    IEnumerator Behavior2()
    {
        if (GetDistanceToPoint(es.gm.Player.transform.position) > AttackRange||GetDistanceToPoint(es.gm.Player.transform.position)<10)
        {
            yield return StartCoroutine(Teleport(GetTeleportPoint()));
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
            if (DroneSpawnCounter >= DroneSpawnCooldown)
            {
                DroneSpawnCounter = 0;
                yield return StartCoroutine(SpawnDrone());
            }
        }
        DroneSpawnCounter += Time.deltaTime;
        IsExecuting = false;
        yield return null;
    }

    Vector2 GetTeleportPoint()
    {
        Vector2 Result = Vector2.zero;
        RaycastHit2D Hit = Physics2D.Raycast(transform.position,transform.up, 1);
        for(int i = 0; i < 4; i++)
        {
            int r = Random.Range(0, 4);
            switch (r)
            {
                case 0:
                    Hit = Physics2D.Raycast(es.gm.Player.transform.position, es.gm.Player.transform.up * -1, 12, LayerMask.GetMask("Walls"));
                    Result = es.gm.Player.transform.position + es.gm.Player.transform.up*-12;
                    break;
                case 1:
                    Hit = Physics2D.Raycast(es.gm.Player.transform.position, es.gm.Player.transform.up, 12, LayerMask.GetMask("Walls"));
                    Result = es.gm.Player.transform.position + es.gm.Player.transform.up* 12;
                    break;
                case 2:
                    Hit = Physics2D.Raycast(es.gm.Player.transform.position, es.gm.Player.transform.right, 12, LayerMask.GetMask("Walls"));
                    Result = es.gm.Player.transform.position + es.gm.Player.transform.right*12;
                    break;
                case 3:
                    Hit = Physics2D.Raycast(es.gm.Player.transform.position, es.gm.Player.transform.right * -1, 12, LayerMask.GetMask("Walls"));
                    Result = es.gm.Player.transform.position + es.gm.Player.transform.right *-12;
                    break;
            }
            if (!Hit)
            {
                PreviousSafeSpot = Result;
                return Result;
            }
        }
        return PreviousSafeSpot;
    }
    IEnumerator Teleport(Vector2 Target)
    {
        SendAnimatorTrigger("IsTeleport");
        es.ac.PlaySound("BeamOut", AudioMaster.SoundTypes.SFX, false,false);
        yield return new WaitForSeconds(0.45f);
        es.SetCollidersEnabled(false);
        transform.position = Target;
        es.ac.PlaySound("BeamIn", AudioMaster.SoundTypes.SFX, false,false);
        yield return new WaitForSeconds(0.45f);
        es.SetCollidersEnabled(true);
    }
    IEnumerator SpawnDrone()
    {
        es.ac.PlaySound("Droid2", AudioMaster.SoundTypes.SFX, true,false);
        SendAnimatorTrigger("IsLaugh");
        yield return new WaitForSeconds(0.4f);
        if(ActiveDrones.Count < 4)
        {
            GameObject Drone = Instantiate(Drones[Random.Range(0, Drones.Length)], (Vector2)transform.position + Random.insideUnitCircle * 2, Quaternion.identity);
            ActiveDrones.Add(Drone.GetComponent<EntityStats>());
        }
    }
    void ClearDrones()
    {
        List<EntityStats> ToRemove = new List<EntityStats>();
        foreach(EntityStats e in ActiveDrones)
        {
            if (e.IsDead)
            {
                ToRemove.Add(e);
            }
        }
        foreach(EntityStats e in ToRemove)
        {
            ActiveDrones.Remove(e);
        }
    }
    public override void SendAnimatorTrigger(string Name)
    {
        AIAnimator.SetTrigger(Name);
        Tank.gameObject.GetComponent<Animator>().SetTrigger(Name);
    }
    IEnumerator ExplosionLights()
    {
        yield return new WaitForSeconds(0.45f);
        ExplosionLight.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        ExplosionLight.SetActive(false);
        yield return null;
    }
    void Explode()
    {
        ExplosionPFX.SetActive(true);
        ExplosionPFX.GetComponentInChildren<AudioController>().PlaySound("AcidExplosion", AudioMaster.SoundTypes.SFX, true,false);
        Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, 5, LayerMask.GetMask("Enemies") + LayerMask.GetMask("Player"));
        foreach (Collider2D c in Hits)
        {
            EntityStats e = c.GetComponentInParent<EntityStats>();
            e.TakeDamage(ExplosionDamage);
            if (!e.IsWeakpoint && !e.IsShield)
            {
                Vector2 Dir = (e.transform.position - transform.position).normalized;
                e.rb.velocity = Dir * 8;
            }
        }
    }
}
