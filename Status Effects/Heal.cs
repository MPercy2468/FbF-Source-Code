using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : StatusEffect
{    
    protected override void ApplyEffect(EntityStats es)
    {
        if(Duration == 0)
        {
            es.AddHealth(Magnitude);
        }
        else
        {
            es.AddHealth(Magnitude * Time.deltaTime);
        }
    }

    protected override void StopEffect(EntityStats es)
    {

    }
}
