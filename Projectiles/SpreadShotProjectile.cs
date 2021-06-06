using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShotProjectile : Projectile
{
    bool wasDamageScaled = false;
    private void Update()
    {
        ProjectileLogic();
        ScaleDamage();
    }
    void ScaleDamage()
    {
        if (wasDamageScaled)
        {
            return;
        }
        if (Vector2.Distance(transform.position, OriginPosition) > 7)
        {
            ProjectileDamage.DamageAmount *= 0.01f;
            wasDamageScaled = true;
        }
    }
}
