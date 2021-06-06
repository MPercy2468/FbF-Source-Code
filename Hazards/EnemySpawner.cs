using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject BeamEffect;
    [Tooltip("Event to be called when all waves are beaten")]
    [SerializeField]
    UnityEvent FinishEvent;
    [Tooltip("Time in seconds between waves")]
    public float TimeBetweenWaves = 0;
    [Tooltip("If enabled, spawner will continuously spawn waves from top to bottom")]
    public bool InfinateSpawns;
    [Tooltip("Array that contains wave data used by the spawner")]
    public EnemyWave[] Waves;
    int WaveIndex = 0;
    List<EntityStats> LiveEnemies = new List<EntityStats>();
    bool WasSpawnerStarted = false,IsWaveSpawned = false;

    [HideInInspector]
    public bool isDisabled;
    private void Update()
    {
        if (IsWaveSpawned)
        {
            CheckLiveEnemies();
            if(LiveEnemies.Count == 0)
            {
                IsWaveSpawned = false;
                WaveIndex++;
                if (WaveIndex >= Waves.Length)
                {
                    if (InfinateSpawns)
                    {
                        Finish();
                        WaveIndex = 0;
                    }
                    else
                    {
                        Finish();
                        return;
                    }
                }
                NextWave();
            }
        }
    }
    void Finish()
    {
        FinishEvent.Invoke();
    }
    void CheckLiveEnemies()
    {
        List<EntityStats> ToRemove = new List<EntityStats>();
        foreach(EntityStats e in LiveEnemies)
        {
            if (e.IsDead)
            {
                ToRemove.Add(e);
            }
        }
        foreach(EntityStats e in ToRemove)
        {
            LiveEnemies.Remove(e);
        }
    }
    public void StartSpawner()
    {
        if (isDisabled)
        {
            Finish();
            return;
        }
        if (WasSpawnerStarted)
        {
            return;
        }
        if(Waves.Length == 0)
        {
            Finish();
            return;
        }
        Debug.Log("Spawner started", this);
        WasSpawnerStarted = true;
        StartCoroutine(SpawnWave(Waves[WaveIndex]));
    }
    void NextWave()
    {
        StartCoroutine(SpawnWave(Waves[WaveIndex]));
    }
    IEnumerator SpawnWave(EnemyWave Wave)
    {
        yield return new WaitForSeconds(TimeBetweenWaves);
        int SpawnIndex = 0;
        for(int i = 0; i < Wave.Enemies.Length; i++)
        {
            if (Wave.SpawnAtRandomPoints)
            {
                GameObject Enemy = Instantiate(Wave.Enemies[i], Wave.SpawnPoints[Random.Range(0,Wave.SpawnPoints.Length)].transform.position, Quaternion.identity);
                GameObject Beam = Instantiate(BeamEffect, Enemy.transform.position, Quaternion.identity);
                Beam.GetComponentInChildren<AudioController>().PlaySound("BeamIn", AudioMaster.SoundTypes.SFX, false,false);
                SpawnIndex++;
                if (SpawnIndex >= Wave.SpawnPoints.Length)
                {
                    SpawnIndex = 0;
                }
                LiveEnemies.Add(Enemy.GetComponentInParent<EntityStats>());
            }
            else
            {
                GameObject Enemy = Instantiate(Wave.Enemies[i], Wave.SpawnPoints[SpawnIndex].transform.position, Quaternion.identity);
                GameObject Beam = Instantiate(BeamEffect, Enemy.transform.position, Quaternion.identity);
                Beam.GetComponentInChildren<AudioController>().PlaySound("BeamIn", AudioMaster.SoundTypes.SFX, false,false);
                SpawnIndex++;
                if (SpawnIndex >= Wave.SpawnPoints.Length)
                {
                    SpawnIndex = 0;
                }
                LiveEnemies.Add(Enemy.GetComponentInParent<EntityStats>());
            }
            yield return new WaitForSeconds(Wave.SpawnDelay);
        }
        IsWaveSpawned = true;
    }
}
