using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : StatusEffect
{
    float AmmoCounter = 1;
    protected override void ApplyEffect(EntityStats es)
    {
        if(Duration == 0)
        {
            es.gm.AddAmmo((int)Magnitude, AmmoType);
        }
        else
        {
            if(AmmoCounter >= 1)
            {
                AmmoCounter = 0;
                es.gm.AddAmmo((int)Magnitude, AmmoType);
            }
            AmmoCounter += Time.deltaTime;
        }
    }

    protected override void StopEffect(EntityStats es)
    {

    }
}
