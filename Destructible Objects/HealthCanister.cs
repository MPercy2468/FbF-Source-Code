using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCanister : DestructableObject
{
    [SerializeField]
    int HealthPickups;
    public override void DestroyObject()
    {
        base.DestroyObject();
        for (int i = 0; i < HealthPickups; i++)
        {
            Vector2 Point = Random.insideUnitCircle;
            GameObject HealthPickup = Instantiate(es.gm.FinisherHealthPickup, (Vector2)transform.position + Point * 2, es.gm.Player.transform.rotation);
        }
    }
}
