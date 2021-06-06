using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pheonix")]
public class BikeChip : OverclockChip
{
    public override void ChipLogic(Overclock o)
    {
        o.isBike = true;
    }
}
