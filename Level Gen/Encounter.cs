using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Encounter
{
    [SerializeField]
	private EnemySpawner spawner;
    [SerializeField]
	private float difficultyWeight;

    public void StartEncounter()
    {
        spawner.StartSpawner();
    }
}
