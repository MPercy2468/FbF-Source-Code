using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    [Tooltip("If enabled, enemies will spawn at random points defined in the SpawnPoints array."+"\n"+
        "Otherwise, spawnpoints will be used in order from top to bottom")]
    public bool SpawnAtRandomPoints;
    [Tooltip("Time in seconds between enemy spawns for this wave")]
    public float SpawnDelay = 0;
    [Tooltip("Array of enemies to be spawned for this wave")]
    public GameObject[] Enemies;
    [Tooltip("Array of spawnpoints for this wave")]
    public Transform[] SpawnPoints;
}
