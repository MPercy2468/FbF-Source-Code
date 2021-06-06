using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Knockback : StatusEffect
{
    protected override void ApplyEffect(EntityStats es)
    {
        es.IsKnockback = true;
        if (es.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            es.gameObject.GetComponent<NavMeshAgent>().velocity = Direction * Magnitude;
        }
        else if (!es.IsShield && !es.IsWeakpoint)
        {
            if (es.rb!=null)
            {
                es.rb.velocity = Direction * Magnitude;
            }
        }
    }
    protected override void StopEffect(EntityStats es)
    {
        es.IsKnockback = false;
        if (es.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            es.gameObject.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
        }
        else if (!es.IsShield && !es.IsWeakpoint)
        {
            if (es.rb != null)
            {
                es.rb.velocity = Vector3.zero;
            }
        }
    }
}
