using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[System.Serializable]
public class Damage
{
    public enum damageTypeTagsEnum
    {
        Neutral,
        Projectile,
        Explosive,
        Hazard,
        Melee
    }
    public enum damageSourceTagsEnum
    {
        Neutral,
        Enemy,
        GreenGun,
        Shotgun,
        Crossbow,
        PlasmaSabre,
        PlasmaSpear,
        PlasmaAxe
    }
    public float DamageAmount;
    public bool CanHealthFinish, CanAmmoFinish,CanPierce,isExplosive;
    public damageTypeTagsEnum damageTypeTag;
    public damageSourceTagsEnum damageSourceTag;
    public Vector2 Direction;
    public List<StatusEffectData> StatusEffects = new List<StatusEffectData>(1);

    public Damage(float InputDamage,bool InputHealthFinish,bool InputAmmoFinish,bool InputCanPierce,bool InputIsExplosive)
    {
        DamageAmount = InputDamage;
        CanHealthFinish = InputHealthFinish;
        CanAmmoFinish = InputAmmoFinish;
        CanPierce = InputCanPierce;
        isExplosive = InputIsExplosive;
        foreach (StatusEffectData s in StatusEffects)
        {
            s.Direction = Direction;
        }
    }
    public Damage(float InputDamage, bool InputHealthFinish, bool InputAmmoFinish, bool InputCanPierce, bool InputIsExplosive, damageTypeTagsEnum damageType, damageSourceTagsEnum damageSource)
    {
        DamageAmount = InputDamage;
        CanHealthFinish = InputHealthFinish;
        CanAmmoFinish = InputAmmoFinish;
        CanPierce = InputCanPierce;
        isExplosive = InputIsExplosive;
        damageTypeTag = damageType;
        damageSourceTag = damageSource;
        foreach (StatusEffectData s in StatusEffects)
        {
            s.Direction = Direction;
        }
    }
    public Damage(Damage d)
    {
        CopyDamageData(d);
    }
    public void CopyDamageData(Damage damageToCopy)
    {
        DamageAmount = damageToCopy.DamageAmount;
        CanHealthFinish = damageToCopy.CanHealthFinish;
        CanAmmoFinish = damageToCopy.CanAmmoFinish;
        CanPierce = damageToCopy.CanPierce;
        isExplosive = damageToCopy.isExplosive;
        damageTypeTag = damageToCopy.damageTypeTag;
        damageSourceTag = damageToCopy.damageSourceTag;
        Direction = damageToCopy.Direction;
        StatusEffects = new List<StatusEffectData>();
        for(int i = 0; i < damageToCopy.StatusEffects.Count; i++)
        {
            StatusEffects.Add(damageToCopy.StatusEffects[i]);
        }
    }
    public void UpdateDirection(Vector2 DirUpdate)
    {
        Direction = DirUpdate;
        foreach (StatusEffectData s in StatusEffects)
        {
            s.Direction = DirUpdate;
        }
    }
}
