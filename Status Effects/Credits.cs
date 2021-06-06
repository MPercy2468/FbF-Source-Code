using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : StatusEffect
{
    protected override void ApplyEffect(EntityStats es)
    {
        LevelData ld = GameObject.Find("_GameMaster").GetComponent<LevelData>();
        ld.scoreData.credits += (int)Magnitude;
        DurationCounter = Duration;
    }

    protected override void StopEffect(EntityStats es)
    {
        Debug.Log(Magnitude + " credits recieved");
    }
}
