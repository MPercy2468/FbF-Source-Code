using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAmmoBox : AmmoBox
{
    // Start is called before the first frame update
    void Start()
    {
        BallisticPickups = 1;
        EnergyPickups = 2;
        SpecialPickups = 1;
    }
}
