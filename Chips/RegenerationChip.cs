using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Regeneration")]
public class RegenerationChip : OverclockChip
{
    float regenTime = 0.5f, regenCounter = 0.5f;
    public override void ChipLogic(Overclock o)
    {
        if(regenCounter >= regenTime)
        {
            regenCounter = 0;
            o.player.es.AddStatusEffect("Heal", regenTime, 3, EntityStats.EntityTypes.None, GameMaster.AmmoTypes.None, Vector2.zero);
        }
        else
        {
            if(regenCounter < regenTime)
            {
                regenCounter += Time.deltaTime;
            }
        }
    }
}
