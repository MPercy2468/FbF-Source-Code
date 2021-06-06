using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectData
{
    public string Name;
    public float Duration;
    public float Magnitude;
    public GameMaster.AmmoTypes AmmoType;
    public EntityStats.EntityTypes EntityType;
    public Vector2 Direction;
}
