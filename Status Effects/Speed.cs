using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : StatusEffect
{
    float ModifiedSpeed;
    protected override void ApplyEffect(EntityStats es)
    {
        Debug.Log("Speed effect applied");
        ModifiedSpeed = es.OriginalEntityWalkSpeed + Magnitude;
        es.EntityWalkSpeed = ModifiedSpeed;
    }

    protected override void StopEffect(EntityStats es)
    {
        Debug.Log("Speed over");
        es.EntityWalkSpeed = es.OriginalEntityWalkSpeed;
    }
}
