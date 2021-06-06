using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StyleRank
{
    public string rankLetter;
    [HideInInspector]
    public int rankIndex;
    public Sprite rankSprite;
    public float rankScoreMaximum;
    public float rankScoreReductionRate;
}
