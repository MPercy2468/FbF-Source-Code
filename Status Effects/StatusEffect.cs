using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    public float Duration = 0;
    protected float DurationCounter = 0;
    public float Magnitude = 0;
    public bool IsEffectOver = false;
    public GameMaster.AmmoTypes AmmoType;
    public EntityStats.EntityTypes EntityType;
    public Vector2 Direction;
    public void RunEffect(EntityStats es)
    {
        if (IsEffectOver)
        {
            return;
        }
        TickCounter();
        ApplyEffect(es);
        SetEffectOver(es);
    }
    protected abstract void ApplyEffect(EntityStats es);
    protected abstract void StopEffect(EntityStats es);
    void TickCounter()
    {
        DurationCounter += 1 * Time.deltaTime;
    }
    void SetEffectOver(EntityStats es)
    {
        if(DurationCounter >= Duration)
        {
            StopEffect(es);
            IsEffectOver = true;
        }
        else
        {
            IsEffectOver = false;
        }
    }
}
