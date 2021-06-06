using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlasmaAxe")]
public class PlasmaAxeChip : WeaponChip
{
    public override IEnumerator Shoot(WeaponStats ws)
    {
        yield return null;
    }
}
