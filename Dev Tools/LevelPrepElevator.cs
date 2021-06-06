using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPrepElevator : Elevator
{
    public override void LoadNextScene()
    {
        SceneMaster sm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        sm.LoadScene(sm.levelGeneratorSceneBuildIndex);
    }
}
