using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAmmoBox : AmmoBox
{
    // Start is called before the first frame update
    void Start()
    {
        BallisticPickups = 3;
        EnergyPickups = 4;
        SpecialPickups = 2;
    }
}
