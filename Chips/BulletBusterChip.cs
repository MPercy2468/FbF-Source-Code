using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Buster")]
public class BulletBusterChip : OverclockChip
{
    public override void ChipLogic(Overclock o)
    {
        o.isBulletBuster = true;
    }
}
