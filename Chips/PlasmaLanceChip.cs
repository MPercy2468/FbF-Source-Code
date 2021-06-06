using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlasmaLance")]
public class PlasmaLanceChip : WeaponChip
{
    public override IEnumerator Shoot(WeaponStats ws)
    {
        yield return null;
    }
}
