using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField]
    Encounter[] encounters;
    Encounter currentEncounter;
    float generationWeight;
    public Transform roomSpawnPoint;
    [SerializeField]
    Elevator elevator;

    [HideInInspector]
    public RoomData roomData;


    public void AssignRoomData(RoomData data)
    {
        roomData = data;
    }
    public void SetWeight(float weight)
    {
        generationWeight = weight;
    }
    public void ModifyWeight(float weight)
    {
        generationWeight += weight;
    }
    public float GetWeight()
    {
        return generationWeight;
    }

    public Encounter GetCurrentEncounter()
    {
        return currentEncounter;
    }

    public Encounter SetRandomEncounter()
    {
        Encounter result;
        int rand = Random.Range(0, encounters.Length);
        result = encounters[rand];
        currentEncounter = result;
        Debug.Log("RESULT: " + result);
        return result;
    }

    public void FinishRoom()
    {
        OpenAllDoors();
    }
    public void DelayedFinishRoom()
    {
        Invoke("FinishRoom", 0.5f);
    }
    public void OpenAllDoors()
    {
        if (elevator != null)
        {
            elevator.OpenElevator();
        }
    }

    public void CloseAllDoors()
    {
        if (elevator != null)
        {
            elevator.CloseElevator();
        }
    }
}
