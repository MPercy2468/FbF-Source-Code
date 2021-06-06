using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public bool canMove = true;
    public float DashSpeed;
    [SerializeField]
    float DashDuration,DashRefreshTime;
    float DashRefreshCounter;
    public int MaxDashes;
    [HideInInspector]
    public int CurrentDashes;
    [SerializeField]
    float MeleeCooldown;
    float MeleeCooldownValue;
    int MeleeSwingValue;
    [HideInInspector]
    public EntityStats es;
    public Overclock overclock;
    public GrappleHook grappleHook;
    public Melee melee;
    [SerializeField]
    Transform MeleeTransform;
    bool IsDeathDone;
    [SerializeField]
    GameObject Scarf, DashEffect, SwordLight;
    [SerializeField]
    GameObject plasmaSurgeProjectile,PurpleOverclockShield;
    float PreviousOverclockShieldHealth;
    bool canChargeMelee;
    float meleeChargeTime,meleeMaxChargeTime = 1.5f;
    private void Awake()
    {
        es = gameObject.GetComponent<EntityStats>();
        MeleeCooldownValue = MeleeCooldown;
        MeleeSwingValue = 1;
        CurrentDashes = MaxDashes;
        PreviousOverclockShieldHealth = PurpleOverclockShield.GetComponent<EntityStats>().EntityHealth;
        DashEffect.GetComponent<ParticleSystem>().enableEmission = false;
    }

    private void Start()
    {
        es.gm.gc.Player.Melee.performed += Context => StartMeleeCoroutine();
        es.gm.gc.Player.Dash.performed += Context => StartDashCoroutine();
        es.gm.gc.Player.PrimaryItem.performed += Context => UsePrimaryItem();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDeathDone)
        {
            return;
        }
        if (!IsDeathDone)
        {
            if (es.IsDead)
            {
                es.ac.PlaySound("QDeath", AudioMaster.SoundTypes.SFX, false, false);
                es.rb.velocity = Vector2.zero;
                Scarf.SetActive(false);
                SwordLight.SetActive(false);
                es.gm.gc.Player.Disable();
                es.gm.ActivePlayerWeapon.gameObject.GetComponent<WeaponStats>().SendAnimatorTrigger("IsDead");
                StopAllCoroutines();
                es.DestroyEntityShadows();
                es.ac.PlaySound("OverclockOff", AudioMaster.SoundTypes.SFX, false, false);
                es.ac.StopSound("OverclockAmbient");
                IsDeathDone = true;
                float TargetRot = Mathf.Atan2(es.LastHitDirection.y, es.LastHitDirection.x);
                TargetRot = Mathf.Rad2Deg * TargetRot;
                transform.rotation = Quaternion.Euler(0,0,TargetRot);
                Invoke("Die", 2f);
                return;
            }
        }
        //Commented out charged melee logic
        /*
        if (canChargeMelee)
        {
            if (meleeChargeTime > 0.25f)
            {
                if (!InputHandler.GetAction(es.gm.gc.Player.Melee))
                {
                    StartChargedMeleeCoroutine();
                }
            }
        }
        */
        RunPlayerCooldowns();
        RotateToMouse();
        GetPlayerMovement();
        RefreshDashes();
        try
        {
            if (es.gm.ActivePlayerWeapon.IsBeingFired && es.GetEntityState() != EntityStats.EntityStates.Dashing)
            {
                es.SetEntityState(EntityStats.EntityStates.Shooting);
            }
        }
        //Catches an exception that can occur in the equipped weapon
        catch(Exception e)
        {
            Debug.Log("No active player weapon assigned");
        }
    }
    void StartMeleeCoroutine()
    {
        if (es.gm.um.UIState != UIMaster.UIStates.PlayerInfo)
        {
            return;
        }
        if(MeleeCooldownValue >= MeleeCooldown)
        {
            canChargeMelee = true;
            MeleeCooldownValue = 0;
            if (es.gm.ActivePlayerWeapon.secondaryChip)
            {
                if (es.gm.ActivePlayerWeapon.secondaryChip.isMeleeChip)
                {
                    if (es.gm.ActivePlayerWeapon.secondaryChip.chipClassName.Equals("PlasmaLanceChip"))
                    {
                        melee.StartCoroutine(melee.StartMelee(this, Melee.MeleeType.Spear));
                        return;
                    }
                    else if (es.gm.ActivePlayerWeapon.secondaryChip.chipClassName.Equals("PlasmaAxeChip"))
                    {
                        melee.StartCoroutine(melee.StartMelee(this, Melee.MeleeType.Axe));
                        return;
                    }
                }
            }
            melee.StartCoroutine(melee.StartMelee(this, Melee.MeleeType.Sabre));
        }
    }
    void StartChargedMeleeCoroutine()
    {
        if (es.gm.um.UIState != UIMaster.UIStates.PlayerInfo)
        {
            return;
        }
        canChargeMelee = false;
        MeleeCooldownValue = 0;
        if (es.gm.ActivePlayerWeapon.secondaryChip)
        {
            if (es.gm.ActivePlayerWeapon.secondaryChip.isMeleeChip)
            {
                if (es.gm.ActivePlayerWeapon.secondaryChip.chipClassName.Equals("PlasmaLanceChip"))
                {
                    melee.StartCoroutine(melee.StartChargedMelee(this, Melee.MeleeType.Spear,meleeChargeTime));
                    return;
                }
                else if (es.gm.ActivePlayerWeapon.secondaryChip.chipClassName.Equals("PlasmaAxeChip"))
                {
                    melee.StartCoroutine(melee.StartChargedMelee(this, Melee.MeleeType.Axe,meleeChargeTime));
                    return;
                }
            }
        }
        melee.StartCoroutine(melee.StartChargedMelee(this, Melee.MeleeType.Sabre,meleeChargeTime));
    }
    void StartDashCoroutine()
    {
        if (overclock.isBike)
        {
            return;
        }
        if(es.GetEntityState() == EntityStats.EntityStates.Dashing)
        {
            return;
        }
        if(CurrentDashes > 0)
        {
            StartCoroutine(Dash());
        }
    }
    
    IEnumerator Dash()
    {
        CurrentDashes--;
        es.ac.PlaySound("Dash", AudioMaster.SoundTypes.SFX, true, false);
        if (!overclock.isOverclockActive)
        {
            es.gm.ChangeVolume(es.gm.DashProfile);
        }
        DashEffect.GetComponent<ParticleSystem>().enableEmission = true;
        es.SetEntityState(EntityStats.EntityStates.Dashing);
        Vector2 Movement = es.gm.gc.Player.Movement.ReadValue<Vector2>().normalized;
        if (Movement == Vector2.zero)
        {
            Movement = transform.up;
        }
        for (float i = 0; i < DashDuration;)
        {
            es.rb.velocity = Movement * DashSpeed;
            i += 1 * Time.deltaTime;
            yield return null;
        }
        es.SetEntityState(EntityStats.EntityStates.Neutral);
        if (!overclock.isOverclockActive)
        {
            es.gm.ChangeVolume(es.gm.NeutralProfile);
        }
        DashEffect.GetComponent<ParticleSystem>().enableEmission = false;
    }

    void GetPlayerMovement()
    {
        if (!canMove)
        {
            return;
        }
        if(es.gm.um.UIState != UIMaster.UIStates.PlayerInfo)
        {
            return;
        }
        if(es.GetEntityState() != EntityStats.EntityStates.Dashing)
        {
            Vector2 Movement = es.gm.gc.Player.Movement.ReadValue<Vector2>().normalized;
            es.rb.velocity = Movement * es.EntityWalkSpeed;
            if (es.rb.velocity != Vector2.zero)
            {
                es.SetEntityState(EntityStats.EntityStates.Walking);
                es.gm.ActivePlayerWeapon.SendAnimatorBool("IsWalk", true);
            }
            else
            {
                es.SetEntityState(EntityStats.EntityStates.Neutral);
                es.gm.ActivePlayerWeapon.SendAnimatorBool("IsWalk", false);
            }
        }
    }
    void GetPlayerVelocityMovement()
    {
        if (es.GetEntityState() != EntityStats.EntityStates.Dashing)
        {
            if(es.gm.gc.Player.Movement.ReadValue<Vector2>().normalized != Vector2.zero)
            {
                Vector2 Movement = es.gm.gc.Player.Movement.ReadValue<Vector2>().normalized;
                es.rb.velocity += Movement * es.EntityWalkSpeed;
                if (es.rb.velocity.magnitude > es.EntityWalkSpeed)
                {
                    es.rb.velocity = Movement * es.EntityWalkSpeed;
                }
            }

            if (es.rb.velocity != Vector2.zero)
            {
                es.SetEntityState(EntityStats.EntityStates.Walking);
                es.gm.ActivePlayerWeapon.SendAnimatorBool("IsWalk", true);
                if (es.gm.gc.Player.Movement.ReadValue<Vector2>().normalized == Vector2.zero)
                {
                    Debug.Log(es.rb.velocity.magnitude);
                    float Slowdown = Mathf.Lerp(es.rb.velocity.magnitude, 0, 0.3f * Time.deltaTime);
                    es.rb.velocity -= es.rb.velocity.normalized * Slowdown;
                }
            }
            else
            {
                es.SetEntityState(EntityStats.EntityStates.Neutral);
                es.gm.ActivePlayerWeapon.SendAnimatorBool("IsWalk", false);
            }
        }
    }
    float PreviousPlayerAngle = 0;
    void RotateToMouse()
    {
        if(es.gm.gc.Player.enabled == false)
        {
            return;
        }
        if (InputHandler.CurrentDevice == InputHandler.Devices.Gamepad)
        {
            if(es.gm.gc.Player.Aim.ReadValue<Vector2>() == Vector2.zero)
            {
                Vector3 NewRot = new Vector3(0, 0,PreviousPlayerAngle);
                transform.eulerAngles = NewRot;
            }
            else
            {
                //Disgusting solution, but it works
                //At the time of writing this, new unity input system always reads default deadzone values in the project settings
                //It ignores deadzone values set in the "processor" section
                if((es.gm.gc.Player.Aim.ReadValue<Vector2>().x > 0.7f || es.gm.gc.Player.Aim.ReadValue<Vector2>().y > 0.7f)||(es.gm.gc.Player.Aim.ReadValue<Vector2>().x < -0.7f || es.gm.gc.Player.Aim.ReadValue<Vector2>().y < -0.7f))
                {
                    Vector2 AimVector = es.gm.gc.Player.Aim.ReadValue<Vector2>();
                    if (Vector2.Distance(AimVector, transform.position) > 0.3f)
                    {
                        float Rot = Mathf.Atan2(AimVector.y, AimVector.x);
                        Rot = Mathf.Rad2Deg * Rot;
                        Rot -= 90;
                        Vector3 NewRot = new Vector3(0, 0, Rot);
                        transform.eulerAngles = NewRot;
                        PreviousPlayerAngle = NewRot.z;
                    }
                }
            }
        }
        else
        {
            Vector2 AimVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);  
            Vector2 ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 TrigVec = (AimVector - ScreenCenter).normalized;
            float Rot = Mathf.Atan2(TrigVec.y, TrigVec.x);
            Rot = Mathf.Rad2Deg * Rot;
            Rot -= 90;
            Vector3 NewRot = new Vector3(0, 0, Rot);
            transform.eulerAngles = NewRot;
            PreviousPlayerAngle = NewRot.z;         
        }
    }
    void RunPlayerCooldowns()
    {
        if(MeleeCooldownValue < MeleeCooldown)
        {
            MeleeCooldownValue += Time.deltaTime;
        }
        if (canChargeMelee)
        {
            if (meleeChargeTime < meleeMaxChargeTime)
            {
                meleeChargeTime += Time.deltaTime;
            }
            else
            {
                meleeChargeTime = meleeMaxChargeTime;
            }
        }
        else
        {
            meleeChargeTime = 0;
        }
    }

    void RefreshDashes()
    {
        if(CurrentDashes < MaxDashes)
        {
            if (DashRefreshCounter >= DashRefreshTime)
            {
                DashRefreshCounter = 0;
                CurrentDashes = MaxDashes;
            }
            DashRefreshCounter += Time.deltaTime;
        }
    }

    void UsePrimaryItem()
    {
        grappleHook.LaunchHook();
    }
    void Die()
    {
        es.gm.LoadDeathScene();
    }
}
