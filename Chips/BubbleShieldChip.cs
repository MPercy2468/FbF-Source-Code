using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bubble Shield")]
public class BubbleShieldChip : OverclockChip
{
    public override void ChipLogic(Overclock o)
    {
        o.isBubbleShield = true;
    }
}
