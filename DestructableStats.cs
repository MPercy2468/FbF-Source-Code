using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableStats : EntityStats
{
    public override void TakeDamage(Damage InputDamage)
    {
        if(InputDamage.damageTypeTag == Damage.damageTypeTagsEnum.Melee)
        {
            ac.PlaySound(EntityHitSound, AudioMaster.SoundTypes.SFX, true, false);
            rb.AddForce(InputDamage.Direction * 1250);
            rb.AddTorque(Random.Range(-50,50));
            return;
        }
        base.TakeDamage(InputDamage);
    }
}
