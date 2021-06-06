using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChiefJusticeAI : AIController
{
    [SerializeField]
    GameObject frontLeftFirePoint, frontRightFirePoint, leftFirePoint, rightFirePoint;
    [SerializeField]
    float retreatRadius;
    [SerializeField]
    Transform[] retreatPoints;
    [SerializeField]
    Damage burstDashDamage;
    [SerializeField]
    GameObject burstDashPFX;
    bool isBattleMode,isBurstDash;
    bool isDropMines;
    [SerializeField]
    GameObject burstDashMine;
    [SerializeField]
    GameObject sideProjectile;
    [SerializeField]
    Damage spinProjectileDamage;
    [SerializeField]
    GameObject bounceProjectile;
    [SerializeField]
    ProjectileHazard launcher1, launcher2, randomizedLauncher;
    [SerializeField]
    GameObject healthbarGameObject;
    [SerializeField]
    Image healthbarFillImage;
    [SerializeField]
    UnityEvent finishEvent;
    int previousRetreatPointIndex;
    int dashMinePositionMultiplier = 0;
    int dashMineCount;
    float dashMinePositionOffset = 0;
    Vector2 startDashPos;
    private void Start()
    {
        healthbarGameObject.SetActive(false);
        es.isDamageImmune = true;
        
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
        //Update healthbar values
        healthbarFillImage.fillAmount = es.EntityHealth / es.EntityHealthMax;
        //Drop Mines if able
        if (isDropMines)
        {
            DropMines();
        }
        if (CurrentBehaviorState == AIBehavirorStates.State2)
        {
            //Send to state 3 if health below 80%
            if(es.EntityHealth < es.EntityHealthMax * 0.8f)
            {
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
            //Send to state 4 if health below 60%
            if (es.EntityHealth < es.EntityHealthMax * 0.6f)
            {
                CurrentBehaviorState = AIBehavirorStates.State4;
                return;
            }
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior3());
            }
        }
        else if (CurrentBehaviorState == AIBehavirorStates.State4)
        {
            //Send to state 5 if health below 20%
            if (es.EntityHealth < es.EntityHealthMax * 0.2f)
            {
                CurrentBehaviorState = AIBehavirorStates.State5;
                return;
            }
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior4());
            }
        }
        else if(CurrentBehaviorState == AIBehavirorStates.State5)
        {
            if (!IsExecuting)
            {
                IsExecuting = true;
                StartCoroutine(Behavior5());
            }
        }
    }
    public void ActivateBoss()
    {
        IsExecuting = false;
        CurrentBehaviorState = AIBehavirorStates.State2;
        if (!IsExecuting)
        {
            IsExecuting = true;
            StartCoroutine(BattleStartup());
        }
    }
    IEnumerator Behavior2()
    {
        if (!isBattleMode)
        {
            IsExecuting = false;
            yield break;
        }
        if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < retreatRadius)
        {
            yield return StartCoroutine(BurstDash(GetRetreatPointFurthestFromPlayer(),false));
            IsExecuting = false;
            yield return null;
        }
        else
        {
            for (float t = 0; t < 2; t += Time.deltaTime)
            {
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                yield return null;
            }
            float r = Random.Range(0, 100);
            if (r < 50)
            {
                yield return StartCoroutine(DualFrontShoot(5));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(), false));
                for (float t = 0; t < 1; t += Time.deltaTime)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    yield return null;
                }
                yield return StartCoroutine(Shoot(Random.Range(3, 8)));
                yield return StartCoroutine(BurstDash(GetRetreatPointFurthestFromPlayer(), false));
            }
            else
            {
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),false));
                yield return StartCoroutine(DualFrontShootRotate(Random.Range(5, 15), es.gm.Player.gameObject));
                yield return StartCoroutine(ShootRotate(6,es.gm.Player.gameObject));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(), false));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(), false));
                for (float t = 0; t < 1; t += Time.deltaTime)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    yield return null;
                }
                yield return StartCoroutine(DualFrontShoot(Random.Range(5, 10)));
            }
            IsExecuting = false;
            yield return null;
        }
    }
    IEnumerator Behavior3()
    {
        if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < retreatRadius)
        {
            yield return StartCoroutine(BurstDash(GetRetreatPointFurthestFromPlayer(),true));
            IsExecuting = false;
            yield return null;
        }
        else
        {
            for (float t = 0; t < 2; t += Time.deltaTime)
            {
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                yield return null;
            }
            yield return StartCoroutine(DualFrontShootRotate(20,es.gm.Player.gameObject));
            float r = Random.Range(0, 100);
            if (r < 50)
            {
                yield return StartCoroutine(ShootRotate(8, es.gm.Player.gameObject));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),true));
                for (float t = 0; t < 1; t += Time.deltaTime)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    yield return null;
                }
                yield return StartCoroutine(DualFrontShootRotate(10, es.gm.Player.gameObject));
            }
            else
            {
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),true));
            }
            IsExecuting = false;
            yield return null;
        }
    }
    IEnumerator Behavior4()
    {
        if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < retreatRadius)
        {
            yield return StartCoroutine(BurstDash(GetRetreatPointFurthestFromPlayer(),false));
            IsExecuting = false;
            yield return null;
        }
        else
        {
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                yield return null;
            }
            yield return StartCoroutine(DualFrontShootRotate(10, es.gm.Player.gameObject));
            float r = Random.Range(0, 100);
            if (r < 80)
            {
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(), true));
                launcher1.StartHazard();
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(), false));
                yield return StartCoroutine(BurstDash(GetRetreatPointFurthestFromPlayer(), false));
                for (float t = 0; t < 2; t += Time.deltaTime)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    yield return null;
                }
                yield return StartCoroutine(BurstDash(GetRetreatPointFurthestFromPlayer(), true));
                yield return StartCoroutine(QuadShootRotate(Random.Range(5, 15), es.gm.Player.gameObject));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(), true));
                float x = Random.Range(3,5);
                for (float t = 0; t < x; t += Time.deltaTime)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    yield return null;
                }
                yield return StartCoroutine(DualFrontShootRotate(Random.Range(10, 15), es.gm.Player.gameObject));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(), true));
            }
            else if(r < 50)
            {
                yield return StartCoroutine(BurstDash(retreatPoints[4], false));
                yield return StartCoroutine(DualSideShoot(Random.Range(10, 20),false));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),true));
                float x = Random.Range(3, 5);
                for (float t = 0; t < x; t += Time.deltaTime)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    yield return null;
                }
            }
            IsExecuting = false;
            yield return null;
        }
    }
    IEnumerator Behavior5()
    {
        if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < retreatRadius)
        {
            yield return StartCoroutine(BurstDash(GetRetreatPointFurthestFromPlayer(),false));
            IsExecuting = false;
            yield return null;
        }
        else
        {
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                yield return null;
            }
            yield return StartCoroutine(QuadShootRotate(20, es.gm.Player.gameObject));
            float r = Random.Range(0, 100);
            if (r < 50)
            {
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),true));
                yield return StartCoroutine(DualSideShoot(Random.Range(20,40),true));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),false));
                yield return StartCoroutine(DualFrontShootRotate(10, es.gm.Player.gameObject));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),true));
            }
            else
            {
                for (float t = 0; t < 1; t += Time.deltaTime)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    yield return null;
                }
                yield return StartCoroutine(ShootRotate(8,es.gm.Player.gameObject));
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),true));
                for (float t = 0; t < 1; t += Time.deltaTime)
                {
                    RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
                    yield return null;
                }
                yield return StartCoroutine(DualFrontShootRotate(10,es.gm.Player.gameObject));
            }
            IsExecuting = false;
            yield return null;
        }
    }
    IEnumerator BattleStartup()
    {
        es.isDamageImmune = false;
        SendAnimatorTrigger("BattleStart");
        es.ac.PlaySound("Servo1", AudioMaster.SoundTypes.SFX, false, false);
        for(float t = 0; t < 1; t+=Time.deltaTime)
        {
            RotateToTarget(es.gm.Player.transform.position, RotateSpeed);
            yield return null;
        }
        healthbarGameObject.SetActive(true);
        SendAnimatorTrigger("FrontArmsShoot");
        for (int i = 0; i < 6; i++)
        {
            RotateToTarget(es.gm.Player.transform.position, RotateSpeed * 20);
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            //Left Shot
            GameObject Shot = Instantiate(AIProjectile, frontLeftFirePoint.transform.position, frontLeftFirePoint.transform.rotation);
            Projectile p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontLeftFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, frontLeftFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Right shot
            Shot = Instantiate(AIProjectile, frontRightFirePoint.transform.position, frontRightFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontRightFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, frontRightFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            yield return new WaitForSeconds(0.05f);
        }
        SendAnimatorTrigger("StopShoot");
        es.ac.PlaySound("Servo2", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.15f);
        isBattleMode = true;
        IsExecuting = false;
        yield return null;
    }
    IEnumerator DualFrontShoot(int shots)
    {
        SendAnimatorTrigger("FrontArmsShoot");
        es.ac.PlaySound("Servo1", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.2f);
        for(int i = 0; i < shots;i++)
        {
            if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < retreatRadius)
            {
                SendAnimatorTrigger("StopShoot");
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),false));
                yield break;
            }
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            //Left Shot
            GameObject Shot = Instantiate(AIProjectile, frontLeftFirePoint.transform.position, frontLeftFirePoint.transform.rotation);
            Projectile p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontLeftFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, frontLeftFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            p.ReflectionNormal = Hit.normal;
            //Right shot
            Shot = Instantiate(AIProjectile, frontRightFirePoint.transform.position, frontRightFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontRightFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, frontRightFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            p.ReflectionNormal = Hit.normal;
            yield return new WaitForSeconds(0.05f);
        }
        SendAnimatorTrigger("StopShoot");
        es.ac.PlaySound("Servo2", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.15f);
        yield return null;
    }
    IEnumerator DualFrontShootRotate(int shots,GameObject rotateTarget)
    {
        SendAnimatorTrigger("FrontArmsShoot");
        es.ac.PlaySound("Servo1", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < shots; i++)
        {
            if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < retreatRadius)
            {
                SendAnimatorTrigger("StopShoot");
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),false));
                yield break;
            }
            RotateToTarget(rotateTarget.transform.position, RotateSpeed);
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            //Left Shot
            GameObject Shot = Instantiate(AIProjectile, frontLeftFirePoint.transform.position, frontLeftFirePoint.transform.rotation);
            Projectile p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontLeftFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, frontLeftFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Right shot
            Shot = Instantiate(AIProjectile, frontRightFirePoint.transform.position, frontRightFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontRightFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, frontRightFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            yield return new WaitForSeconds(0.05f);
        }
        SendAnimatorTrigger("StopShoot");
        es.ac.PlaySound("Servo2", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.15f);
        yield return null;
    }
    IEnumerator DualSideShoot(int shots,bool isBounce)
    {
        GameObject target = frontLeftFirePoint;
        es.ac.PlaySound("Servo1", AudioMaster.SoundTypes.SFX, false, false);
        int r = Random.Range(0, 10);
        if(r < 5)
        {
            target = frontRightFirePoint;
        }
        SendAnimatorTrigger("SideArmsShoot");
        yield return new WaitForSeconds(0.2f);
        int bounceCount = 0;
        for (int i = 0; i < shots; i++)
        {
            RotateToTarget(target.transform.position, 80);
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            GameObject Shot = null;
            //Left Shot
            if (isBounce)
            {
                if(bounceCount == 2)
                {
                    Shot = Instantiate(bounceProjectile, leftFirePoint.transform.position, leftFirePoint.transform.rotation);
                }
                else
                {
                    Shot = Instantiate(sideProjectile, leftFirePoint.transform.position, leftFirePoint.transform.rotation);
                }
            }
            else
            {
                Shot = Instantiate(sideProjectile, leftFirePoint.transform.position, leftFirePoint.transform.rotation);
            }
            Projectile p = Shot.GetComponent<Projectile>();
            p.rb.velocity = leftFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = spinProjectileDamage;
            p.HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, leftFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Right shot
            if (isBounce)
            {
                if (bounceCount == 2)
                {
                    Shot = Instantiate(bounceProjectile, rightFirePoint.transform.position, rightFirePoint.transform.rotation);
                    bounceCount = 0;
                }
                else
                {
                    Shot = Instantiate(sideProjectile, rightFirePoint.transform.position, rightFirePoint.transform.rotation);
                    bounceCount++;
                }
            }
            else
            {
                Shot = Instantiate(sideProjectile, rightFirePoint.transform.position, rightFirePoint.transform.rotation);
            }
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = rightFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = spinProjectileDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, rightFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            yield return new WaitForSeconds(0.1f);
        }
        SendAnimatorTrigger("StopShoot");
        es.ac.PlaySound("Servo2", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.15f);
        yield return null;
    }
    IEnumerator QuadShoot(int shots)
    {
        SendAnimatorTrigger("AllArmsShoot");
        es.ac.PlaySound("Servo1", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < shots; i++)
        {
            if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < retreatRadius)
            {
                SendAnimatorTrigger("StopShoot");
                yield return StartCoroutine(BurstDash(GetRandomRetreatPoint(),false));
                yield break;
            }
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            //Front Left Shot
            GameObject Shot = Instantiate(AIProjectile, frontLeftFirePoint.transform.position, frontLeftFirePoint.transform.rotation);
            Projectile p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontLeftFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, frontLeftFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Front Right shot
            Shot = Instantiate(AIProjectile, frontRightFirePoint.transform.position, frontRightFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontRightFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, frontRightFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Left Shot
            Shot = Instantiate(sideProjectile, leftFirePoint.transform.position, leftFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = leftFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = spinProjectileDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, leftFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Right shot
            Shot = Instantiate(sideProjectile, rightFirePoint.transform.position, rightFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = rightFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = spinProjectileDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, rightFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            yield return new WaitForSeconds(0.05f);
        }
        SendAnimatorTrigger("StopShoot");
        es.ac.PlaySound("Servo2", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.15f);
        yield return null;
    }
    IEnumerator QuadShootRotate(int shots, GameObject target)
    {
        SendAnimatorTrigger("AllArmsShoot");
        es.ac.PlaySound("Servo1", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < shots; i++)
        {
            RotateToTarget(target.transform.position,RotateSpeed);
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            //Front Left Shot
            GameObject Shot = Instantiate(AIProjectile, frontLeftFirePoint.transform.position, frontLeftFirePoint.transform.rotation);
            Projectile p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontLeftFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, frontLeftFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Front Right shot
            Shot = Instantiate(AIProjectile, frontRightFirePoint.transform.position, frontRightFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = frontRightFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, frontRightFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Left Shot
            Shot = Instantiate(sideProjectile, leftFirePoint.transform.position, leftFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = leftFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = spinProjectileDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, leftFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            //Right shot
            Shot = Instantiate(sideProjectile, rightFirePoint.transform.position, rightFirePoint.transform.rotation);
            p = Shot.GetComponent<Projectile>();
            p.rb.velocity = rightFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = spinProjectileDamage;
            p.HitPlayer = true;
            Hit = Physics2D.Raycast(transform.position, rightFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            yield return new WaitForSeconds(0.05f);
        }
        SendAnimatorTrigger("StopShoot");
        es.ac.PlaySound("Servo2", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.15f);
        yield return null;
    }
    IEnumerator Shoot(int Shots)
    {
        SendAnimatorTrigger("OneArmShoot");
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < Shots;i++)
        {
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            GameObject Shot = Instantiate(AIProjectile, AIFirePoint.transform.position, AIFirePoint.transform.rotation);
            Projectile p = Shot.GetComponent<Projectile>();
            p.rb.velocity = AIFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, AIFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            yield return new WaitForSeconds(0.05f);
        }
        SendAnimatorTrigger("StopShoot");
        es.ac.PlaySound("Servo2", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.15f);
    }
    IEnumerator ShootRotate(int Shots,GameObject target)
    {
        SendAnimatorTrigger("OneArmShoot");
        es.ac.PlaySound("Servo1", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < Shots; i++)
        {
            es.ac.PlaySound("GunShot2", AudioMaster.SoundTypes.SFX, true, false);
            RotateToTarget(target.transform.position, RotateSpeed);
            GameObject Shot = null;
            Shot = Instantiate(AIProjectile, AIFirePoint.transform.position, AIFirePoint.transform.rotation);
            Projectile p = Shot.GetComponent<Projectile>();
            p.rb.velocity = AIFirePoint.transform.up * p.ProjectileSpeed;
            p.ProjectileDamage = AIDamage;
            p.HitPlayer = true;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, AIFirePoint.transform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            yield return new WaitForSeconds(0.05f);
        }
        SendAnimatorTrigger("StopShoot");
        es.ac.PlaySound("Servo2", AudioMaster.SoundTypes.SFX, false, false);
        yield return new WaitForSeconds(0.15f);
    }
    IEnumerator BurstDash(Transform target, bool isMines)
    {
        if (!isBurstDash)
        {
            startDashPos = transform.position;
            es.ac.PlaySound("JusticeDash", AudioMaster.SoundTypes.SFX, true, false);
            Agent.isStopped = false;
            Instantiate(burstDashPFX, transform.position, Quaternion.identity).SetActive(true);
            isBurstDash = true;
            SendAnimatorBool("BurstDash", true);
            if (Vector2.Distance(transform.position, es.gm.Player.transform.position) < 3)
            {
                es.gm.Player.es.TakeDamage(burstDashDamage);
            }
            WalkToPoint(target.position, es.EntityWalkSpeed);
            if (isMines)
            {
                int d = Random.Range(2, 6);
                dashMinePositionOffset = Vector2.Distance(startDashPos, target.position) / d;
                dashMineCount = d;
                isDropMines = true;
            }
            yield return new WaitForSeconds(0.5f);
            SendAnimatorBool("BurstDash", false);
            isBurstDash = false;
            Agent.isStopped = true;
        }
    }
    Transform GetRandomRetreatPoint()
    {
        int r = Random.Range(0, retreatPoints.Length);
        if (r == previousRetreatPointIndex)
        {
            if(r != retreatPoints.Length-1)
            {
                r++;
            }
        }
        return retreatPoints[r];
    }
    Transform GetRetreatPointFurthestFromPlayer()
    {
        Transform result = retreatPoints[0];
        for(int i = 0; i < retreatPoints.Length; i++)
        {
            if (Vector2.Distance(retreatPoints[i].position, es.gm.Player.transform.position) > Vector2.Distance(result.position, es.gm.Player.transform.position))
            {
                result = retreatPoints[i];
            }
        }
        return result;
    }
    Transform GetRetreatPointClosestToPlayer()
    {
        Transform result = retreatPoints[0];
        for (int i = 0; i < retreatPoints.Length; i++)
        {
            if (Vector2.Distance(retreatPoints[i].position, es.gm.Player.transform.position) < Vector2.Distance(result.position, es.gm.Player.transform.position))
            {
                result = retreatPoints[i];
            }
        }
        return result;
    }
    void DropMines()
    {
        if (dashMinePositionMultiplier >= dashMineCount)
        {
            dashMinePositionMultiplier = 0;
            dashMineCount = 0;
            isDropMines = false;
            return;
        }
        if (Vector2.Distance(startDashPos, transform.position) > Vector2.Distance(startDashPos, startDashPos + (Vector2)AIFirePoint.transform.up*dashMinePositionOffset*dashMinePositionMultiplier))
        {
            Instantiate(burstDashMine, transform.position, Quaternion.identity).GetComponent<EntityStats>().ac.PlaySound("MineArm",AudioMaster.SoundTypes.SFX,true,false);
            dashMinePositionMultiplier++;
        }
    }
    public override void RunAIDeath()
    {
        healthbarGameObject.SetActive(false);
        finishEvent.Invoke();
        healthbarFillImage.fillAmount = 0;
        StopAllCoroutines();
        Agent.isStopped = true;
        Agent.velocity = Vector2.zero;
        Destroy(Agent);
        es.DestroyEntityShadows();
        es.rb.velocity = Vector2.zero;
        AIGFXRenderer.sortingLayerName = "GroundEffects";
        SendAnimatorTrigger("IsDeath");
        gameObject.AddComponent<GibGFX>().renderers = es.EntitySpriteRenderers;
        IsDeathDone = true;
    }
}
