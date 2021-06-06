using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageResistance : DamageModifier
{
    public override Damage ModifyDamage(Damage damage)
    {
        Damage result = new Damage(damage);
        if(result.damageSourceTag == damageSource)
        {
            float mod = 1 - sourceModifer;
            if (mod < 0)
            {
                mod = 0;
            }
            result.DamageAmount *= mod;
        }
        if(result.damageTypeTag == damageType)
        {
            float mod = 1 - typeModifer;
            if (mod < 0)
            {
                mod = 0;
            }
            result.DamageAmount *= mod;
        }
        return result;
    }
}
