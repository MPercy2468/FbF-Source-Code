using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricHazard : Hazard
{
    [SerializeField]
    ElectricTile[] electricTiles;
    bool isEnd;
    private void Awake()
    {
        foreach (ElectricTile e in electricTiles)
        {
            e.eh = this;
        }
    }
    public override void StartHazard()
    {
        hazardState = hazardStatesEnum.on;
        foreach (ElectricTile e in electricTiles)
        {
            e.TurnOn();
        }
    }

    public override void StopHazard()
    {
        hazardState = hazardStatesEnum.off;
        foreach (ElectricTile e in electricTiles)
        {
            e.TurnOff();
        }
        if (isEnd)
        {
            hazardState = hazardStatesEnum.neutral;
            return;
        }
        if (canRepeat)
        {
            Invoke("StartHazard", timeToStart);
        }
    }

    public override void EndHazard()
    {
        isEnd = true;
    }
    public override void HazardUpdateLogic()
    {

    }
    public void RecieveDamageSignal(EntityStats e)
    {
        if(hazardState == hazardStatesEnum.on)
        {
            if (canDamage)
            {
                canDamage = false;
                e.TakeDamage(hazardDamage);
                ResetDamageTickCounter();
            }
        }
    }
}
