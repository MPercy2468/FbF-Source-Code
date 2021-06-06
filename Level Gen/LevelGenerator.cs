using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    //Array for keeping references to room libraries for each sector
    [SerializeField]
    private RoomLibrary[] roomLibraries;
    //Index for room libraries
    [HideInInspector]
    public int sectorIndex;
    //Array for keeping values of generated rooms, [floorIndex]
    private RoomData[] generatedRooms;
    //Index for current floor
    [HideInInspector]
    public int floorIndex = 0;

    //bool to see if the level generator has to load a room or not
    [HideInInspector]
    public bool hasLoadRoomOrder = true;

    //If true, will generate the assigned room for testing purposes
    [SerializeField]
    [Tooltip("If true, will generate the assigned room for testing purposes")]
    bool isTestRoom;
    [SerializeField]
    Room roomToTest;
    //if true, will generate a set of rooms from the given scene
    [SerializeField]
    bool isTestGeneration;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        if (isTestRoom)
        {
            LoadRoom(roomToTest);
        }
        else if(isTestGeneration)
        {
            GenerateFloors(4);
            LoadRoom(floorIndex);
        }
    }

    public void ExecuteRoomLoadOrder()
    {
        if (!hasLoadRoomOrder)
        {
            return;
        }
        LoadRoom(floorIndex);
        hasLoadRoomOrder = false;
    }
    
    public void GenerateSector(int newSectorIndex)
    {
        sectorIndex = newSectorIndex;
        GenerateFloors(4);
    }
    public void GenerateFloors(int floors)
    {
        RoomLibrary library = roomLibraries[sectorIndex];
        generatedRooms = new RoomData[floors];
        for(int i = 0; i < floors; i++)
        {
            RoomData room = new RoomData();
            if (i == floors - 1)
            {
                room.isBossRoom = true;
            }
            else
            {
                room.roomPrefab = library.GetRandomRoom();
                generatedRooms[i] = room;
            }
        }
    }

    public void LoadRoom(int floor)
    {
        if (generatedRooms[floor].isBossRoom)
        {
            SceneMaster sm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
            sm.LoadScene(roomLibraries[sectorIndex].bossSceneIndex);
        }
        else
        {
            Room roomCopy = generatedRooms[floor].roomPrefab;
            GameMaster gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
            //Spawn room from generated rooms array
            GameObject g = Instantiate(roomCopy.gameObject, Vector3.zero, Quaternion.identity);
            roomCopy = g.GetComponent<Room>();
            roomCopy.AssignRoomData(generatedRooms[floor]);
            //Rebuild navmesh
            gm.RebuildNavmesh();
            //Start random encounter
            roomCopy.SetRandomEncounter().StartEncounter();
            Debug.Log("Loaded room at... Floor: " + floor);
        }
    }
    public void LoadRoom(Room room)
    {
        GameMaster gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
        //Spawn room from generated rooms array
        GameObject g = Instantiate(room.gameObject, Vector3.zero, Quaternion.identity);
        room = g.GetComponent<Room>();
        //Rebuild navmesh
        gm.RebuildNavmesh();
        //Start random encounter
        room.SetRandomEncounter().StartEncounter();
    }

    int IterateIndexBounded(int arrayLength,int currentIndex,int iterateAmount)
    {
        int result = currentIndex + iterateAmount;
        if(result >= arrayLength)
        {
            result = (result - arrayLength);
        }
        return result;
    }
}
