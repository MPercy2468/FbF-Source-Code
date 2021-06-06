using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OverclockChip : Chip
{
    [SerializeField]
    protected float abilityDrain;
    public abstract void ChipLogic(Overclock o);
}
