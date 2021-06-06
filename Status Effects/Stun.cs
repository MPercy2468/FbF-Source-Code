using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : StatusEffect
{
    protected override void ApplyEffect(EntityStats es)
    {
        if(EntityType == EntityStats.EntityTypes.None)
        {
            es.IsStunned = true;
        }
        else
        {
            if(es.EntityType == EntityType)
            {
                es.IsStunned = true;
            }
        }
    }

    protected override void StopEffect(EntityStats es)
    {
        es.IsStunned = false;
    }
}
