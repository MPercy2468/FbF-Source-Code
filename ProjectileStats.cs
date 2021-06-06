using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStats : EntityStats
{
    public override void KillEntity()
    {
        if (IsDead)
        {
            return;
        }
        IsDead = true;
    }
}
