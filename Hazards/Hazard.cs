using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    protected enum hazardStatesEnum
    {
        neutral,
        off,
        on
    }
    protected hazardStatesEnum hazardState = hazardStatesEnum.neutral;
    [SerializeField]
    protected AudioController ac;
    [SerializeField]
    protected Damage hazardDamage;
    float damageTickCounter;
    [SerializeField]
    protected bool canRepeat;
    [SerializeField]
    protected bool isSelfStart;
    [SerializeField]
    protected float timeToStart;
    [SerializeField]
    protected bool hasDuration;
    [SerializeField]
    protected float hazardDurationTime;
    protected float hazardDurationCounter;
    protected bool canDamage = true;
    public abstract void StartHazard();
    public abstract void StopHazard();
    public abstract void EndHazard();
    public abstract void HazardUpdateLogic();
    private void Start()
    {
        if (isSelfStart)
        {
            StartHazard();
        }
    }
    protected void DamageTick()
    {
        if (!canDamage)
        {
            if(damageTickCounter >= 0.35f)
            {
                canDamage = true;
            }
            damageTickCounter += Time.deltaTime;
        }
    }
    public virtual void DurationTick()
    {
        if (!hasDuration)
        {
            return;
        }
        if(hazardState == hazardStatesEnum.on)
        {
            if(hazardDurationCounter >= hazardDurationTime)
            {
                hazardDurationCounter = 0;
                StopHazard();
            }
            hazardDurationCounter += Time.deltaTime;
        }
    }
    private void Update()
    {
        DurationTick();
        DamageTick();
        HazardUpdateLogic();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDamage)
        {
            collision.gameObject.GetComponent<EntityStats>().TakeDamage(hazardDamage);
            damageTickCounter = 0;
            canDamage = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canDamage)
        {
            collision.gameObject.GetComponent<EntityStats>().TakeDamage(hazardDamage);
            damageTickCounter = 0;
            canDamage = false;
        }
    }
    protected void ResetDamageTickCounter()
    {
        damageTickCounter = 0;
    }
}
