using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChipEquipData
{
    public Chip[] equippedChips;
    public ChipEquipData(Chip[] inputChips)
    {
        equippedChips = inputChips;
    }
}
