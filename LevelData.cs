using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelData : MonoBehaviour
{
    public ScoreData scoreData;
    ScoreMaster scm;
    private void Start()
    {
        scm = GameObject.Find("_ScoreMaster").GetComponent<ScoreMaster>();
        scoreData = scm.scoreData;
    }

    private void Update()
    {
        CountLevelTime();
    }
    void CountLevelTime()
    {
        scm.scoreData.timeToComplete += Time.unscaledDeltaTime;
    }
}
