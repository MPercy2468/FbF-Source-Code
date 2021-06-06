using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverclockProfile
{
    public OverclockChip[] overclockChips = new OverclockChip[4];
    public void RunOverclockEffects(Overclock overclock)
    {
        overclock.ModifyOverclockCharge(-13 * Time.deltaTime);
        foreach(OverclockChip c in overclockChips)
        {
            if (c != null)
            {
                c.ChipLogic(overclock);
            }
        }
    }
}
