using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIController : MonoBehaviour
{
    [SerializeField]
    GameObject beamEffect;
    [HideInInspector]
    public EntityStats es;
    protected NavMeshAgent Agent;
    [SerializeField]
    protected bool isStationary;
    [SerializeField]
    protected float SightRange,AttackRange,RotateSpeed;
    [SerializeField]
    [Tooltip("The amount of time the AI will wait before attempting to follow the player after not seeing them")]
    protected float SightTimeTolerance;
    protected float SightTimeCounter;
    protected bool IsExecuting,IsDeathDone;
    protected enum AIBehavirorStates
    {
        State1,State2,State3,State4,State5,Beam
    }
    protected AIBehavirorStates CurrentBehaviorState;
    [SerializeField]
    protected GameObject PatrolPoints;
    protected LayerMask PlayerAndWallsMask;
    [SerializeField]
    protected Animator AIAnimator;
    [SerializeField]
    protected SpriteRenderer AIGFXRenderer;
    public GameObject AIFirePoint;
    [SerializeField]
    protected GameObject AIProjectile;
    [SerializeField]
    protected Damage AIDamage;
    protected LineRenderer attackLine;
    protected float timeToBeamCounter;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Door>() != null)
        {
            Debug.Log("Aye bruh, this is a door");
            CurrentBehaviorState = AIBehavirorStates.Beam;
        }
    }

    private void Awake()
    {
        
        es = gameObject.GetComponent<EntityStats>();
        Agent = gameObject.GetComponent<NavMeshAgent>();
        PlayerAndWallsMask = LayerMask.GetMask("Player") + LayerMask.GetMask("Walls");
        attackLine = gameObject.GetComponent<LineRenderer>();
    }
    protected float t = 0;       
    protected void RotateToTarget(Vector2 Target, float InputSpeed)
    {
        Vector2 TrigVec =(Vector2)transform.position - Target;
        float TargetRot = Mathf.Atan2(TrigVec.y, TrigVec.x);
        TargetRot = Mathf.Rad2Deg * TargetRot;
        Quaternion QuatRot = Quaternion.Euler(TargetRot, -90, 90);
        transform.rotation = Quaternion.Lerp(transform.rotation, QuatRot, Time.deltaTime *InputSpeed);
    }

    protected bool CanSeePlayer()
    {
        Vector2 PlayerDir = (es.gm.Player.transform.position - transform.position).normalized;
        RaycastHit2D Hit = Physics2D.Raycast(transform.position, PlayerDir,SightRange,PlayerAndWallsMask);
        //Debug.DrawRay(transform.position, PlayerDir*SightRange, Color.red);
        if(!Hit)
        {
            return false;
        }
        else if (Hit.transform.gameObject.GetComponent<Player>() != null)
        {
            return true;
        }
        return false;
    }
    protected bool CanSeePlayerSightTime()
    {
        Vector2 PlayerDir = (es.gm.Player.transform.position - transform.position).normalized;
        RaycastHit2D Hit = Physics2D.Raycast(transform.position, PlayerDir, SightRange, PlayerAndWallsMask);
        //Debug.DrawRay(transform.position, PlayerDir*SightRange, Color.red);
        if (!Hit)
        {
            return false;
        }
        else if (Hit.transform.gameObject.GetComponent<Player>() != null)
        {
            return true;
        }

        if (SightTimeCounter >= SightTimeTolerance)
        {
            SightTimeCounter = 0;
            return false;
        }
        else
        {
            SightTimeCounter += Time.deltaTime;
            return true;
        }
    }
    protected bool IsPlayerInRange()
    {
        if(Vector2.Distance(es.gm.Player.transform.position, transform.position) < AttackRange)
        {
            return true;
        }

        return false;
    }
    public void WalkToPoint(Vector2 TargetPoint,float WalkSpeed)
    {
        if (isStationary)
        {
            return;
        }
        Agent.speed = WalkSpeed;
        Agent.SetDestination(TargetPoint);
    }
    protected void LeadTarget(GameObject Target, float LeadDistance,float LeadSpeed)
    {
        Vector2 TargetPosition = (Vector2)Target.transform.position + Target.GetComponent<Rigidbody2D>().velocity.normalized * LeadDistance;
        RotateToTarget(TargetPosition,LeadSpeed);
    }
    public virtual void SendAnimatorBool(string Name, bool Value)
    {
        AIAnimator.SetBool(Name, Value);
    }
    public virtual void SendAnimatorInt(string Name, int Value)
    {
        AIAnimator.SetInteger(Name, Value);
    }
    public virtual void SendAnimatorTrigger(string Name)
    {
        AIAnimator.SetTrigger(Name);
    }
    protected float GetDistanceToPoint(Vector2 Point)
    {
        float Result = Vector2.Distance(transform.position, Point);
        return Result;
    }
    public virtual void RunAIDeath()
    {
        StopAllCoroutines();
        EraseAttackLine();
        Agent.isStopped = true;
        Agent.velocity = Vector2.zero;
        Destroy(Agent);
        es.DestroyEntityShadows();
        es.rb.velocity = Vector2.zero;
        AIGFXRenderer.sortingLayerName = "GroundEffects";
        if (es.LastDamageTaken.isExplosive && es.gs != null)
        {
            es.DestroyEntitySpriteRenderers();
            es.gs.ExplodeGibs(es.LastDamageTaken.Direction);
        }
        else
        {
            SendAnimatorTrigger("IsDeath");
            float TargetRot = Mathf.Atan2(es.LastHitDirection.y, es.LastHitDirection.x);
            TargetRot = Mathf.Rad2Deg * TargetRot;
            transform.rotation = Quaternion.Euler(TargetRot, -90, 90);
        }
        gameObject.AddComponent<GibGFX>().renderers = es.EntitySpriteRenderers;
        es.gm.GetComponent<LevelData>().scoreData.killCount++;
        IsDeathDone = true;
    }
    public virtual void RetreatFromPoint(GameObject point,float retreatDistance)
    {
        Vector2 dir = ((Vector2)transform.position - (Vector2)point.transform.position).normalized;
        Vector2 Result = (Vector2)point.transform.position + dir * retreatDistance;
        RaycastHit2D Hit = Physics2D.Raycast(point.transform.position,dir,retreatDistance,LayerMask.GetMask("Walls"));
        if (Hit)
        {
            Result = Hit.point;
        }
        WalkToPoint(Result, es.EntityWalkSpeed);
    }

    public virtual void DrawAttackLine(float lineLength)
    {
        attackLine.enabled = true;
        Vector3 startPos = AIFirePoint.transform.position;
        Vector3 endPos = startPos + AIFirePoint.transform.up * lineLength;
        RaycastHit2D hit = Physics2D.Raycast(AIFirePoint.transform.position, AIFirePoint.transform.up, lineLength, LayerMask.GetMask("Walls"));
        if (hit)
        {
            endPos = hit.point;
        }
        Vector3 midPos = startPos + AIFirePoint.transform.up * (Vector3.Distance(startPos, endPos) / 2);
        attackLine.SetPosition(0, startPos);
        attackLine.SetPosition(1, midPos);
        attackLine.SetPosition(2, endPos);
    }
    public virtual void EraseAttackLine()
    {
        attackLine.enabled = false;
    }
    public virtual IEnumerator BeamToPlayer()
    {
        Agent.isStopped = true;
        Instantiate(beamEffect, transform.position, Quaternion.identity);
        foreach(SpriteRenderer r in es.EntitySpriteRenderers)
        {
            if(r != null)
            {
                r.enabled = false;
            }
        }
        yield return new WaitForSeconds(1f);
        Vector2 newPosition = (Vector2)es.gm.Player.transform.position + Random.insideUnitCircle * 12;
        Vector2 rayDir = ((Vector2)es.gm.Player.transform.position - newPosition).normalized;
        RaycastHit2D hit = Physics2D.Raycast(es.gm.Player.transform.position, rayDir, 12, LayerMask.GetMask("Walls"));
        if (hit)
        {
            newPosition = hit.point;
        }
        transform.position = newPosition;
        Instantiate(beamEffect, transform.position, Quaternion.identity);
        foreach(SpriteRenderer r in es.EntitySpriteRenderers)
        {
            if(r != null)
            {
                r.enabled = true;
            }
        }
        Agent.isStopped = false;
        CurrentBehaviorState = AIBehavirorStates.State1;
        IsExecuting = false;
        yield return null;
    }
}
