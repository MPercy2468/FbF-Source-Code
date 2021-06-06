using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaOrbStats : EntityStats
{
    public override void KillEntity()
    {
        if (IsDead)
        {
            return;
        }
        IsDead = true;
    }

    public override void TakeDamage(Damage InputDamage)
    {
        base.TakeDamage(InputDamage);
        GetComponentInParent<PlasmaOrbProjectile>().AugmentDamage(InputDamage);
    }
}
