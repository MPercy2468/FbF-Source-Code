using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Speed Boost")]
public class SpeedChip : OverclockChip
{
    float speedTime = 0.5f, speedCounter = 0.5f;
    public override void ChipLogic(Overclock o)
    {
        if (speedCounter >= speedTime)
        {
            speedCounter = 0;
            o.player.es.AddStatusEffect("Speed", speedTime, 10, EntityStats.EntityTypes.None, GameMaster.AmmoTypes.None, Vector2.zero);
        }
        else
        {
            if (speedCounter < speedTime)
            {
                speedCounter += Time.deltaTime;
            }
        }
    }
}
