using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDroneAI : AIController
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
                es.ac.PlaySound("Droid1", AudioMaster.SoundTypes.SFX, true,false);
                Agent.isStopped = true;
                Agent.velocity = Vector2.zero;
                Destroy(Agent);
                SendAnimatorTrigger("IsDeath");
                StopAllCoroutines();
                es.DestroyEntityShadows();
                es.rb.velocity = Vector2.zero;
                IsDeathDone = true;
                es.gm.GetComponent<LevelData>().scoreData.killCount++;
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
        if (GetDistanceToPoint(es.gm.Player.transform.position) > AttackRange||!CanSeePlayer())
        {
            WalkToPoint(es.gm.Player.transform.position,es.EntityWalkSpeed);
        }
        else
        {
            for(float i = 0; i < 0.5f; i += Time.deltaTime)
            {
                DrawAttackLine(30);
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
            }
            yield return StartCoroutine(Shoot(1));
        }
        IsExecuting = false;
        yield return null;
    }
    IEnumerator Shoot(int Shots)
    {
        EraseAttackLine();
        for (int i = 0; i < Shots;)
        {
            es.ac.PlaySound("RocketLaunch", AudioMaster.SoundTypes.SFX, true,false);
            GameObject Shot = Instantiate(AIProjectile, AIFirePoint.transform.position, AIFirePoint.transform.rotation);
            Shot.GetComponent<Projectile>().ProjectileSpeed = 25;
            Shot.GetComponent<Projectile>().rb.velocity = AIFirePoint.transform.up * Shot.GetComponent<Projectile>().ProjectileSpeed;
            Shot.GetComponent<Projectile>().ProjectileDamage = AIDamage;
            Shot.GetComponent<Projectile>().HitPlayer = true;
            Shot.GetComponent<Projectile>().IsExplosive = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, AIFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            Shot.GetComponent<Projectile>().WallPosition = Hit.point;
            yield return new WaitForSeconds(0.5f);
            i++;
        }
        yield return new WaitForSeconds(1.5f);
    }
}
