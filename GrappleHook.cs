using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    GameObject hookPrefab;
    GameObject hook;
    public GameObject target;
    GameMaster gm;
    bool isAllowHook = true;
    bool isHookPull,isHookShot;
    LineRenderer lr;
    float stationaryTimeCounter;
    Vector2 previousPosition;
    bool isAmbient;
    private void Awake()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
        lr = gameObject.GetComponent<LineRenderer>();
    }
    private void Start()
    {
        gm.gc.Player.Dash.performed += Context => StopPull();
        lr.enabled = false;

        previousPosition = transform.position;
    }
    public void LaunchHook()
    {
        if (isAllowHook)
        {
            if (gm.Player.overclock.GetCurrentOverclockCharge() < 28)
            {
                return;
            }
            gm.Player.es.ac.PlaySound("ChainLaunch", AudioMaster.SoundTypes.SFX, true, false);
            gm.Player.overclock.ModifyOverclockCharge(-28);
            isAllowHook = false;
            hook = Instantiate(hookPrefab, transform.position, transform.rotation);
            hook.GetComponent<Projectile>().rb.velocity = transform.up * 70;
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, transform.up, 400, LayerMask.GetMask("Walls"));
            hook.GetComponent<Projectile>().WallPosition = Hit.point;
            isHookShot = true;
        }
        else
        {
            StopPull();
        }
    }
    public void PullPlayerToTarget()
    {
        if (!isAmbient)
        {
            isAmbient = true;
            gm.Player.es.ac.PlaySound("ChainAmbient", AudioMaster.SoundTypes.SFX, false, true);
        }
        isHookPull = true;
        Vector2 pullDir = ((Vector2)target.transform.position - (Vector2)gm.Player.transform.position).normalized;
        gm.Player.es.SetEntityState(EntityStats.EntityStates.Dashing);
        gm.Player.es.rb.velocity = pullDir * 45;
        gm.Player.es.isDamageImmune = true;
    }
    public void PullTargetToPlayer()
    {
        isHookPull = true;
        Vector2 pullDir = ((Vector2)gm.Player.transform.position - (Vector2)target.transform.position).normalized;
        if (target.GetComponent<EntityStats>() != null)
        {
            EntityStats es = target.GetComponent<EntityStats>();
            es.AddStatusEffect("Stun", 0.3f, 0, EntityStats.EntityTypes.None, GameMaster.AmmoTypes.None, Vector2.zero);
            target.GetComponent<EntityStats>().rb.velocity = pullDir * 45;
        }
        else
        {
            StopPull();
        }
    }
    void RenderChain()
    {
        lr.enabled = true;
        lr.SetPosition(0, gm.Player.transform.position);
        lr.SetPosition(1, hook.transform.position);
    }
    void StartPull(GameObject Target)
    {
        target = Target;
        isHookPull = true;
    }
    public void StopPull()
    {
        isAmbient = false;
        gm.Player.es.ac.StopSound("ChainAmbient");
        Debug.Log("Stop pull");
        isAllowHook = true;
        lr.enabled = false;
        isHookPull = false;
        isHookShot = false;
        Destroy(hook);
        hook = null;
        gm.Player.es.isDamageImmune = false;
        gm.Player.es.SetEntityState(EntityStats.EntityStates.Neutral);
    }
    void CheckStationary()
    {
        if (Vector2.Distance(transform.position, previousPosition) < 0.5f)
        {
            if (stationaryTimeCounter >= 0.5f)
            {
                stationaryTimeCounter = 0;
                StopPull();
            }
            else
            {
                stationaryTimeCounter += Time.deltaTime;
            }
        }
        else
        {
            previousPosition = transform.position;
            stationaryTimeCounter = 0;
        }
    }
    private void Update()
    {
        if (isHookShot)
        {
            RenderChain();
            if (isHookPull)
            {
                PullPlayerToTarget();
                CheckStationary();
                if (Vector2.Distance(gm.Player.transform.position, target.transform.position) < 1)
                {
                    StopPull();
                }
            }
        }
    }
}
