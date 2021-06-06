using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DamageModifier
{
    public Damage.damageSourceTagsEnum damageSource;
    public Damage.damageTypeTagsEnum damageType;
    public float sourceModifer;
    public float typeModifer;
    public abstract Damage ModifyDamage(Damage damage);
}
