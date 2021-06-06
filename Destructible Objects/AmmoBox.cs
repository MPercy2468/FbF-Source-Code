using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : DestructableObject
{
    protected int BallisticPickups, EnergyPickups, SpecialPickups;
    public override void DestroyObject()
    {
        base.DestroyObject();
        GameObject AmmoPickup = null;
        for (int i = 0; i < BallisticPickups; i++)
        {
            Vector2 Point = Random.insideUnitCircle;
            AmmoPickup = Instantiate(es.gm.FinisherBallisticAmmoPickup, (Vector2)transform.position + Point * 2, es.gm.Player.transform.rotation);
        }
        for (int i = 0; i < EnergyPickups; i++)
        {
            Vector2 Point = Random.insideUnitCircle;
            AmmoPickup = Instantiate(es.gm.FinisherEnergyAmmoPickup, (Vector2)transform.position + Point * 2, es.gm.Player.transform.rotation);
        }
        for (int i = 0; i < SpecialPickups; i++)
        {
            Vector2 Point = Random.insideUnitCircle;
            AmmoPickup = Instantiate(es.gm.FinisherSpecialAmmoPickup, (Vector2)transform.position + Point * 2, es.gm.Player.transform.rotation);
        }
    }
}
