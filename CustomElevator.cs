using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomElevator : Elevator
{
    [SerializeField]
    int indexToLoad;
    public override void LoadNextScene()
    {
        SceneManager.LoadScene(indexToLoad);
    }
}
