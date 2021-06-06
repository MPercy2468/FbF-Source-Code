using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelGenerator lg = GameObject.Find("_LevelGenerator").GetComponent<LevelGenerator>();
        lg.ExecuteRoomLoadOrder();
    }
}
