using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PeacekeeperAI : AIController
{
    [SerializeField]
    GameObject LeftGun, RightGun;
    [SerializeField]
    GameObject LeftAIFirePoint,RightAIFirePoint;
    [SerializeField]
    EntityStats LeftWeakpoint, RightWeakpoint;
    [SerializeField]
    GameObject RedLight, BlueLight, ExplosionLight;
    [SerializeField]
    Damage ExplosionDamage;
    [SerializeField]
    GameObject ExplosionPFX;
    [SerializeField]
    LineRenderer attackLine2;
    bool AreLightsRunning;
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
                EraseAttackLine();
                RedLight.SetActive(false);
                BlueLight.SetActive(false);
                Agent.isStopped = true;
                Agent.velocity = Vector2.zero;
                Destroy(Agent);
                AIGFXRenderer.sortingLayerName = "GroundEffects";
                StopAllCoroutines();
                StartCoroutine(ExplosionLights());
                es.DestroyEntityShadows();
                SendAnimatorTrigger("IsDeath");
                if(LeftGun != null)
                {
                    LeftWeakpoint.GetComponent<Animator>().SetTrigger("IsDeath");
                    Destroy(LeftGun);
                }
                if(RightGun != null)
                {
                    RightWeakpoint.GetComponent<Animator>().SetTrigger("IsDeath");
                    Destroy(RightGun);
                }
                IsDeathDone = true;
                StartCoroutine(ExplosionLights());
                Invoke("Explode", 0.25f);
                es.gm.GetComponent<LevelData>().scoreData.killCount++;
                return;
            }
            else if(LeftGun == null && RightGun == null)
            {
                EraseAttackLine();
                es.RemoveHealth(1000);
                RedLight.SetActive(false);
                BlueLight.SetActive(false);
                Agent.isStopped = true;
                Destroy(Agent);
                SendAnimatorBool("IsDead", true);
                AIGFXRenderer.sortingLayerName = "GroundEffects";
                StopAllCoroutines();
                es.DestroyEntityShadows();
                IsDeathDone = true;
                float TargetRot = Mathf.Atan2(es.LastHitDirection.y, es.LastHitDirection.x);
                TargetRot = Mathf.Rad2Deg * TargetRot;
                transform.rotation = Quaternion.Euler(TargetRot, -90, 90);
                StartCoroutine(ExplosionLights());
                Invoke("Explode", 0.25f);
                es.gm.GetComponent<LevelData>().scoreData.killCount++;
                return;
            }

            if (LeftWeakpoint.IsDead)
            {
                if (LeftGun != null)
                {
                    LeftWeakpoint.GetComponent<Animator>().SetTrigger("IsDeath");
                    RightWeakpoint.GetComponent<Animator>().SetTrigger("IsShake");
                    SendAnimatorTrigger("IsShakeRight");
                    Destroy(LeftGun);
                }
            }
            if (RightWeakpoint.IsDead)
            {
                if (RightGun != null)
                {
                    RightWeakpoint.GetComponent<Animator>().SetTrigger("IsDeath");
                    LeftWeakpoint.GetComponent<Animator>().SetTrigger("IsShake");
                    SendAnimatorTrigger("IsShakeLeft");
                    Destroy(RightGun);
                }
            }

        }
        if (!AreLightsRunning)
        {
            StartCoroutine(AlternateLights());
        }
        if (es.IsKnockback)
        {
            if (LeftGun != null)
            {
                LeftGun.GetComponent<Animator>().SetBool("IsShoot", false);
            }
            if(RightGun != null)
            {
                RightGun.GetComponent<Animator>().SetBool("IsShoot", false);
            }
            IsExecuting = false;
            StopAllCoroutines();
            return;
        }
        if (CurrentBehaviorState == AIBehavirorStates.State1)
        {
            if (CanSeePlayer())
            {
                es.ac.PlaySound("RobotOnline", AudioMaster.SoundTypes.SFX, true, false);
                IsExecuting = false;
                CurrentBehaviorState = AIBehavirorStates.State2;
                return;
            }
        }
        else if (CurrentBehaviorState == AIBehavirorStates.State2)
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
        if (es.IsStunned)
        {
            IsExecuting = false;
            yield break;
        }

        if (!IsPlayerInRange()|| !CanSeePlayer()||!CanGunsSeePlayer())
        {
            if (es.IsStunned)
            {
                IsExecuting = false;
                yield break;
            }
            Agent.isStopped = false;
            WalkToPoint(es.gm.Player.transform.position,es.EntityWalkSpeed);
        }
        else
        {
            Agent.isStopped = true;
            Agent.velocity = Vector2.zero;
            //Rotates to the player for two seconds
            for (float i = 0; i < 2;)
            {
                if (es.IsStunned)
                {
                    IsExecuting = false;
                    yield break;
                }
                DrawAttackLine(AttackRange);
                RotateToTarget(es.gm.Player.transform.position,RotateSpeed);
                RotateGunsToPlayer();
                if (!CanSeePlayer())
                {
                    break;
                }
 
                i += 1 * Time.deltaTime;
                yield return null;
            }
            yield return StartCoroutine(Shoot(9));
        }
        IsExecuting = false;
        yield return null;
    }
    IEnumerator Behavior3()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector2.zero;
        yield return null;
    }
    IEnumerator Shoot(int Shots)
    {
        Vector2 PlayerDir = (es.gm.Player.transform.position - transform.position).normalized;
        Vector2 PlayerVec = es.gm.Player.transform.position;
        Vector2 TrigVec = (Vector2)transform.position - PlayerVec;
        float Rot = Mathf.Atan2(TrigVec.y, TrigVec.x);
        Rot = Mathf.Rad2Deg * Rot;
        int CanSeeCounter = 0;
        for (int i = 0; i < Shots;)
        {
            if (!CanSeePlayer())
            {
                CanSeeCounter++;
            }
            if(CanSeeCounter > 3)
            {
                EraseAttackLine();
                yield break;
            }
            if (es.IsStunned)
            {
                EraseAttackLine();
                yield break;
            }
            if (i < 3 || i == 6 || i == 8)
            {
                if (RightGun != null)
                {
                    EraseAttackLine();
                    RaycastHit2D wallCheck = Physics2D.Raycast(RightAIFirePoint.transform.position, RightAIFirePoint.transform.up, 
                        Vector2.Distance(transform.position, es.gm.Player.transform.position), LayerMask.GetMask("Walls"));
                    if (!wallCheck)
                    {
                        try
                        {
                            RightGun.GetComponent<Animator>().SetBool("IsShoot", true);
                            GameObject Shot = Instantiate(AIProjectile, RightAIFirePoint.transform.position, RightAIFirePoint.transform.rotation);
                            Shot.GetComponent<Projectile>().rb.velocity = RightAIFirePoint.transform.up * Shot.GetComponent<Projectile>().ProjectileSpeed;
                            Shot.GetComponent<Projectile>().ProjectileDamage = AIDamage;
                            Shot.GetComponent<Projectile>().HitPlayer = true;
                            RaycastHit2D Hit = Physics2D.Raycast(RightAIFirePoint.transform.position, RightAIFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
                            Shot.GetComponent<Projectile>().WallPosition = Hit.point;
                            es.ac.PlaySound("PKCannon", AudioMaster.SoundTypes.SFX, true, false);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Right gun exception 1");
                        }
                        yield return new WaitForSeconds(0.9f);
                        try
                        {
                            RightGun.GetComponent<Animator>().SetBool("IsShoot", false);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Right gun exception 2");
                        }
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
            else if (i < 6 || i == 7)
            {
                if (LeftGun != null)
                {
                    EraseAttackLine();
                    RaycastHit2D wallCheck = Physics2D.Raycast(LeftAIFirePoint.transform.position, LeftAIFirePoint.transform.up,
                        Vector2.Distance(transform.position, es.gm.Player.transform.position), LayerMask.GetMask("Walls"));
                    if (!wallCheck)
                    {
                        try
                        {
                            LeftGun.GetComponent<Animator>().SetBool("IsShoot", true);
                            GameObject Shot = Instantiate(AIProjectile, LeftAIFirePoint.transform.position, LeftAIFirePoint.transform.rotation);
                            Shot.GetComponent<Projectile>().rb.velocity = LeftAIFirePoint.transform.up * Shot.GetComponent<Projectile>().ProjectileSpeed;
                            Shot.GetComponent<Projectile>().ProjectileDamage = AIDamage;
                            Shot.GetComponent<Projectile>().HitPlayer = true;
                            RaycastHit2D Hit = Physics2D.Raycast(LeftAIFirePoint.transform.position, LeftAIFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
                            Shot.GetComponent<Projectile>().WallPosition = Hit.point;
                            es.ac.PlaySound("PKCannon", AudioMaster.SoundTypes.SFX, true, false);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Left gun exception 1");
                        }
                        yield return new WaitForSeconds(0.9f);
                        try
                        {

                            LeftGun.GetComponent<Animator>().SetBool("IsShoot", false);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Left gun exception 2");
                        }
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
            for (float t = 0; t < 0.5f;)
            {
                if (es.IsStunned)
                {
                    IsExecuting = false;
                    yield break;
                }
                DrawAttackLine(AttackRange);
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                RotateGunsToPlayer();
                if (!CanSeePlayer())
                {
                    EraseAttackLine();
                    break;
                }
                t += 1 * Time.deltaTime;
                yield return null;
            }
            i++;
        }
    }
    protected void RotateGunsToPlayer()
    {
        Vector2 PlayerVec = es.gm.Player.transform.position;
        if (RightGun != null)
        {
            Vector2 RightTrigVec = (Vector2)RightGun.transform.position - PlayerVec;
            float RightTargetRot = Mathf.Atan2(RightTrigVec.y, RightTrigVec.x);
            RightTargetRot = Mathf.Rad2Deg * RightTargetRot;
            Quaternion RightQuatRot = Quaternion.Euler(0, 0, RightTargetRot + 90);
            RightGun.transform.rotation = Quaternion.Lerp(RightGun.transform.rotation, RightQuatRot, Time.deltaTime * 10);
        }
        if(LeftGun != null)
        {
            Vector2 LeftTrigVec = (Vector2)LeftGun.transform.position - PlayerVec;
            float LeftTargetRot = Mathf.Atan2(LeftTrigVec.y, LeftTrigVec.x);
            LeftTargetRot = Mathf.Rad2Deg * LeftTargetRot;
            Quaternion LeftQuatRot = Quaternion.Euler(0, 0, LeftTargetRot + 90);
            LeftGun.transform.rotation = Quaternion.Lerp(LeftGun.transform.rotation, LeftQuatRot, Time.deltaTime * 10);
        }
    }

    bool CanGunsSeePlayer()
    {
        bool CanLeft = false, CanRight = false;
        if (LeftGun != null)
        {
            Vector2 PlayerDirLeft = (es.gm.Player.transform.position - LeftGun.transform.position).normalized;
            RaycastHit2D HitLeft = Physics2D.Raycast(LeftGun.transform.position, PlayerDirLeft, AttackRange, PlayerAndWallsMask);
            Debug.DrawRay(LeftGun.transform.position, PlayerDirLeft * SightRange, Color.red);
            if (HitLeft)
            {
                CanLeft = true;
            }
            if(RightGun == null)
            {
                if (CanLeft)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        if(RightGun != null)
        {
            Vector2 PlayerDirRight = (es.gm.Player.transform.position - RightGun.transform.position).normalized;
            RaycastHit2D HitRight = Physics2D.Raycast(RightGun.transform.position, PlayerDirRight, AttackRange, PlayerAndWallsMask);
            Debug.DrawRay(RightGun.transform.position, PlayerDirRight * SightRange, Color.red);
            if (HitRight)
            {
                CanRight = true;
            }
        }
        if(LeftGun == null && RightGun == null)
        {
            return false;
        }
        else if(LeftGun == null)
        {
            if (CanRight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(RightGun == null)
        {
            if (CanLeft)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (CanLeft && CanRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    IEnumerator AlternateLights()
    {
        AreLightsRunning = true;
        RedLight.SetActive(false);
        BlueLight.SetActive(true);
        yield return new WaitForSeconds(1);
        RedLight.SetActive(true);
        BlueLight.SetActive(false);
        yield return new WaitForSeconds(1);
        yield return null;
        AreLightsRunning = false;
    }
    IEnumerator ExplosionLights()
    {
        yield return new WaitForSeconds(0.3f);
        ExplosionLight.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        ExplosionLight.SetActive(false);
        yield return null;
    }
    void Explode()
    {
        es.DestroyEntitySpriteRenderers();
        ExplosionPFX.SetActive(true);
        ExplosionPFX.GetComponentInChildren<AudioController>().PlaySound("HEExplosion", AudioMaster.SoundTypes.SFX, true,false);
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
        es.gs.ExplodeGibs(Vector2.zero);
    }
    public override void DrawAttackLine(float lineLength)
    {
        Vector3 startPos = Vector2.zero;
        Vector3 endPos = Vector2.zero;
        RaycastHit2D hit = new RaycastHit2D();
        Vector3 midPos = Vector2.zero;
        //Left gun attack line
        if (LeftGun)
        {
            attackLine.enabled = true;
            startPos = LeftAIFirePoint.transform.position;
            endPos = startPos + LeftAIFirePoint.transform.up * lineLength;
            hit = Physics2D.Raycast(LeftAIFirePoint.transform.position, LeftAIFirePoint.transform.up, lineLength, LayerMask.GetMask("Walls"));
            if (hit)
            {
                endPos = hit.point;
            }
            midPos = startPos + LeftAIFirePoint.transform.up * (Vector3.Distance(startPos, endPos) / 2);
            attackLine.SetPosition(0, startPos);
            attackLine.SetPosition(1, midPos);
            attackLine.SetPosition(2, endPos);
        }

        //Right gun attack line
        if (RightGun)
        {
            attackLine2.enabled = true;
            startPos = RightAIFirePoint.transform.position;
            endPos = startPos + RightAIFirePoint.transform.up * lineLength;
            hit = Physics2D.Raycast(RightAIFirePoint.transform.position, RightAIFirePoint.transform.up, lineLength, LayerMask.GetMask("Walls"));
            if (hit)
            {
                endPos = hit.point;
            }
            midPos = startPos + RightAIFirePoint.transform.up * (Vector3.Distance(startPos, endPos) / 2);
            attackLine2.SetPosition(0, startPos);
            attackLine2.SetPosition(1, midPos);
            attackLine2.SetPosition(2, endPos);
        }
    }
    public override void EraseAttackLine()
    {
        base.EraseAttackLine();
        attackLine2.enabled = false;
    }
}
