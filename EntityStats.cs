using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Class for containing an entity's information
//Not all variables are used by every entity
public class EntityStats : MonoBehaviour
{
    //Reference for this entity's parent
    //Used for weakpoints and shields
    public EntityStats esParent;
    //Enums for tracking this entity's state
    [HideInInspector]
    public enum EntityStates
    {
        Neutral,Walking,Dashing,Shooting,Melee
    }
    EntityStates EntityState;
    //Enums for tracking this entity's properties
    [HideInInspector]
    public enum EntityTypes
    {
        Organic,Cybernetic,None
    }
    public EntityTypes EntityType;
    //Floats for tracking this entity's health and overclock values
    public float EntityHealth;
    [HideInInspector]
    public float EntityHealthMax;
    float EntityFinisherThreshold;
    //Float for tracking this entity's walk speed
    public float EntityWalkSpeed;
    [HideInInspector]
    public float OriginalEntityWalkSpeed;
    //List for handling different status effects
    public List<StatusEffect> AppliedStatusEffects;
    [SerializeField]
    List<DamageModifier> damageModifiers = new List<DamageModifier>();
    [SerializeField]
    int HealthPickups, BallisticPickups,EnergyPickups,SpecialPickups;
    public bool IsPlayer,IsShield,IsWeakpoint,IsDrone,HasDrone,IsDead;
    public bool isCritKillable;
    [HideInInspector]
    public bool IsStunned, IsKnockback,isDamageImmune;

    public SpriteRenderer[] EntitySpriteRenderers;
    public SpriteRenderer[] ShadowSpriteRenderers;

    [HideInInspector]
    public GameMaster gm;
    public AudioController ac;
    [SerializeField]
    protected string EntityHitSound, EntityDeathSound;

    [HideInInspector]
    public Rigidbody2D rb;

    [SerializeField]
    Collider2D[] Colliders;
    public Vector2 LastHitDirection;
    public Damage LastDamageTaken;

    [HideInInspector]
    public GibSystem gs;


    bool CanBeFinished,WasHealthFinished, WasAmmoFinished;
    float FinisherDuration = 5, FinisherCounter = 0;

    private void Awake()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        gs = gameObject.GetComponent<GibSystem>();
        EntityHealthMax = EntityHealth;
        EntityFinisherThreshold = EntityHealthMax * 0.25f;
        OriginalEntityWalkSpeed = EntityWalkSpeed;
    }
    private void Update()
    {
        if (IsDead)
        {
            return;
        }
        CalculateStatusEffects();
        if (EntityHealth <= 0)
        {
            KillEntity();
            return;
        }
        if(EntityHealth > EntityHealthMax)
        {
            EntityHealth = EntityHealthMax;
        }
        SetCritKillState();
        SetMaterial();
    }
    void SetCritKillState()
    {
        if (!isCritKillable)
        {
            return;
        }
        if (EntityHealth <= EntityFinisherThreshold)
        {
            if (!IsPlayer)
            {
                if (!CanBeFinished)
                {
                    CanBeFinished = true;
                }
                else if (CanBeFinished)
                {
                    if (FinisherCounter >= FinisherDuration)
                    {
                        EntityHealth = EntityFinisherThreshold + 1;
                        CanBeFinished = false;
                    }
                }
            }
        }
        else if (EntityHealth > EntityFinisherThreshold)
        {
            CanBeFinished = false;
        }
    }
    public virtual void KillEntity()
    {
        if (IsDead)
        {
            return;
        }
        if (CanBeFinished)
        {
            FinisherKill(WasHealthFinished, WasAmmoFinished);
        }
        foreach (SpriteRenderer s in EntitySpriteRenderers)
        {
            if(s != null)
            {
                s.material = gm.DefaultMaterial;
            }
        }
        ac.PlaySound(EntityDeathSound, AudioMaster.SoundTypes.SFX, true,false);
        DestroyColliders();
        gm.um.sm.IncreaseStyleScore(200, StyleMeter.styleScoreMultiplierEnum.EnemyKilled);
        IsDead = true;
    }
    public void FinisherKill(bool HealthFinish, bool AmmoFinish)
    {
        if (HealthFinish || AmmoFinish)
        {
            ac.PlaySound("FinisherKill", AudioMaster.SoundTypes.SFX, false, false);
            gm.um.sm.IncreaseStyleScore(400, StyleMeter.styleScoreMultiplierEnum.CriticalKilled);
            gm.ld.scoreData.criticalKillCount++;
        }
        if (HealthFinish)
        {
            for (int i = 0; i < HealthPickups; i++)
            {
                Vector2 Point = Random.insideUnitCircle;
                GameObject HealthPickup = Instantiate(gm.FinisherHealthPickup, (Vector2)transform.position + Point * 2, gm.Player.transform.rotation);
            }
        }
        if (AmmoFinish)
        {
            GameObject AmmoPickup = null;
            for (int i = 0; i < BallisticPickups; i++)
            {
                Vector2 Point = Random.insideUnitCircle;
                AmmoPickup = Instantiate(gm.FinisherBallisticAmmoPickup, (Vector2)transform.position + Point * 2, gm.Player.transform.rotation);
            }
            for (int i = 0; i < EnergyPickups; i++)
            {
                Vector2 Point = Random.insideUnitCircle;
                AmmoPickup = Instantiate(gm.FinisherEnergyAmmoPickup, (Vector2)transform.position + Point * 2, gm.Player.transform.rotation);
            }
            for (int i = 0; i < SpecialPickups; i++)
            {
                Vector2 Point = Random.insideUnitCircle;
                AmmoPickup = Instantiate(gm.FinisherSpecialAmmoPickup, (Vector2)transform.position + Point * 2, gm.Player.transform.rotation);
            }
        }
    }
    void CalculateStatusEffects()
    {
        if (AppliedStatusEffects.Count == 0)
        {
            return;
        }
        List<StatusEffect> StatusEffectsToRemove = new List<StatusEffect>();
        for (int i = 0; i < AppliedStatusEffects.Count; i++)
        {
            if (!AppliedStatusEffects[i].IsEffectOver)
            {
                AppliedStatusEffects[i].RunEffect(this);
            }
            else
            {
                StatusEffectsToRemove.Add(AppliedStatusEffects[i]);
            }
        }
        for(int i = 0; i < StatusEffectsToRemove.Count; i++)
        {
            AppliedStatusEffects.Remove(StatusEffectsToRemove[i]);
        }
    }
    //Method for adding satus effects to this entity
    public void AddStatusEffect(string effectName, float inputDuration, float inputMagnitude, EntityTypes inputEntityType, GameMaster.AmmoTypes inputAmmoType, Vector2 inputDirection)
    {
        StatusEffect Status = (StatusEffect)ScriptableObject.CreateInstance(effectName);
        Status.Duration = inputDuration;
        Status.Magnitude = inputMagnitude;
        Status.EntityType = inputEntityType;
        Status.AmmoType = inputAmmoType;
        Status.Direction = inputDirection;
        AppliedStatusEffects.Add(Status);
    }
    public virtual void TakeDamage(Damage InputDamage)
    {
        //Instaniate new damage class for calculation
        InputDamage = new Damage(InputDamage);
        //Check resistances and weaknesses
        foreach(DamageModifier m in damageModifiers)
        {
            InputDamage = m.ModifyDamage(InputDamage);
        }
        //Drain overclock if shield is activated
        if(IsPlayer&&gm.Player.overclock.isBubbleShield == true)
        {
            gm.Player.overclock.ModifyOverclockCharge(InputDamage.DamageAmount*2);
            return;
        }
        //Return if entity is immune to damage
        if (isDamageImmune)
        {
            return;
        }
        ac.PlaySound(EntityHitSound, AudioMaster.SoundTypes.SFX, true,false);
        if (IsShield)
        {
            if (InputDamage.CanPierce)
            {
                esParent.TakeDamage(InputDamage);
            }
        }
        LastHitDirection = InputDamage.Direction;
        LastDamageTaken = InputDamage;
        if(gs != null)
        {
            gs.SplatterBlood(2);
        }
        foreach(StatusEffectData s in InputDamage.StatusEffects)
        {
            AddStatusEffect(s.Name, s.Duration, s.Magnitude,s.EntityType,s.AmmoType,s.Direction);
        }
        if (CanBeFinished)
        {
            if (InputDamage.CanHealthFinish)
            {
                WasHealthFinished = true;

            }
            if (InputDamage.CanAmmoFinish)
            {
                WasAmmoFinished = true;
            }
            if (InputDamage.CanHealthFinish || InputDamage.CanAmmoFinish)
            {
                if (!gm.Player.overclock.GetIsOverClockActive())
                {
                    gm.Player.overclock.ModifyOverclockCharge(100);
                }
                KillEntity();
                return;
            }
        }
        if(!gm.Player.overclock.GetIsOverClockActive())
        {
            gm.Player.overclock.ModifyOverclockCharge(InputDamage.DamageAmount / 4);
        }
        if (IsPlayer)
        {
            gm.um.sm.DecreaseStyleScore(100);
        }
        RemoveHealth(InputDamage.DamageAmount);
    }

    public void AddHealth(float Amount)
    {
        EntityHealth += Amount;
    }
    public void RemoveHealth(float Amount)
    {
        EntityHealth -= Amount;
    }
    public void DestroyColliders()
    {
        for(int i = 0; i < Colliders.Length;i++)
        {
            if(Colliders[i] != null)
            {
                Destroy(Colliders[i]);
            }
        }
    }
    public void SetCollidersEnabled(bool Input)
    {
        for (int i = 0; i < Colliders.Length; i++)
        {
            if (Colliders[i] != null)
            {
                Colliders[i].enabled = Input;
            }
        }
    }
    void SetMaterial()
    {
        if (!IsPlayer)
        {
            if (CanBeFinished)
            {
                foreach (SpriteRenderer s in EntitySpriteRenderers)
                {
                    if(s != null)
                    {
                        s.material = gm.FinisherMatirial;
                    }
                }
            }
            else if (IsStunned)
            {
                foreach (SpriteRenderer s in EntitySpriteRenderers)
                {
                    if (s != null)
                    {
                        s.material = gm.StunMaterial;
                    }
                }
            }
            else
            {
                foreach (SpriteRenderer s in EntitySpriteRenderers)
                {
                    if (s != null)
                    {
                        s.material = gm.DefaultMaterial;
                    }
                }
            }
        }
    }
    public void DestroyEntityShadows()
    {
        foreach (SpriteRenderer s in ShadowSpriteRenderers)
        {
            if (s != null)
            {
                Destroy(s.gameObject);
            }
        }
    }
    public void DestroyEntitySpriteRenderers()
    {
        foreach (SpriteRenderer s in EntitySpriteRenderers)
        {
            if (s != null)
            {
                Destroy(s.gameObject);
            }
        }
    }
    public void SetEnabledEntitySpriteRenderers(bool value)
    {
        foreach(SpriteRenderer s in EntitySpriteRenderers)
        {
            if (s != null)
            {
                s.enabled = value;
            }
        }
    }
    public void CheckpointRemoveEntity()
    {
        DestroyColliders();
        DestroyEntityShadows();
        DestroyEntitySpriteRenderers();
    }

    public void SetEntityState(EntityStates state)
    {
        EntityState = state;
    }

    public EntityStates GetEntityState()
    {
        return EntityState;
    }

    public bool CheckMatchingCollider(Collider2D col)
    {
        for(int i = 0; i < Colliders.Length; i++)
        {
            if(col == Colliders[i])
            {
                return true;
            }
        }
        return false;
    }
}
