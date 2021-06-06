using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Status effect for bonus damage against a specific type
public class BonusDamage : StatusEffect
{
    protected override void ApplyEffect(EntityStats es)
    {
        if(Duration == 0)
        {
            if (es.EntityType == EntityType||EntityType == EntityStats.EntityTypes.None)
            {
                es.RemoveHealth(Magnitude);
            }
        }
        else
        {
            if (es.EntityType == EntityType||EntityType == EntityStats.EntityTypes.None)
            {
                es.RemoveHealth(Magnitude * Time.deltaTime);
            }
        }
    }

    protected override void StopEffect(EntityStats es)
    {

    }
}
