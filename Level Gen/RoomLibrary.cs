using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomLibrary
{
    public int bossSceneIndex;
    [SerializeField]
    Room[] rooms;
    public Room GetRandomRoom()
    {
        return rooms[Random.Range(0, rooms.Length)];
    }

}
