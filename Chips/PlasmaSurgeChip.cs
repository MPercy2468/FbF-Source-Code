using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plasma Surge")]
public class PlasmaSurgeChip : OverclockChip
{
    public override void ChipLogic(Overclock o)
    {
        o.isPlasmaSurge = true;
    }
}
