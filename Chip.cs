using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class Chip : ScriptableObject
{
    public enum chipTypeEnum
    {
        GreenGun,
        Shotgun,
        Crossbow,
        Overclock
    }
    public chipTypeEnum chipType;
    public string chipClassName;
    public int chipID;
    public Sprite chipSprite;
    [TextArea]
    public string chipDescripton;
    public int buyCost = 0;
    public int upgradeCost = 0;
    public bool isUnlocked;
    public bool isUpgraded;
    public void UpgradeChip()
    {
        if (isUpgraded)
        {
            return;
        }
        isUpgraded = true;
    }
}
