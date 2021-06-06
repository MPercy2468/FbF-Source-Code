using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelCompleteScene : MonoBehaviour
{
    ScoreData endData;
    [SerializeField]
    TextMeshProUGUI killsText;
    [SerializeField]
    TextMeshProUGUI critKillsText;
    [SerializeField]
    TextMeshProUGUI timeText;
    ScoreMaster scm;
    ChipMaster cm;
    private void Awake()
    {
        scm = GameObject.Find("_ScoreMaster").GetComponent<ScoreMaster>();
        cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
        endData = scm.scoreData;
    }
    void Start()
    {
        DisplayKillCounts();
        DisplayLevelTime();
    }
    void DisplayAverageScores()
    {

    }
    void DisplayKillCounts()
    {
        killsText.text = endData.killCount.ToString();
        critKillsText.text = endData.criticalKillCount.ToString();
    }
    void DisplayLevelTime()
    {
        float levelTime = endData.timeToComplete;
        float levelMinutes = 0;
        float levelSeconds = 0;
        while (levelTime >= 60)
        {
            levelTime -= 60;
            levelMinutes++;
        }
        levelSeconds = levelTime;
        levelSeconds = Mathf.RoundToInt(levelSeconds);
        if (levelSeconds < 10)
        {
            timeText.text = levelMinutes + ":0" + levelSeconds;
        }
        else
        {
            timeText.text = levelMinutes + ":" + levelSeconds;
        }
    }

    public void NextLevelClicked()
    {
        SceneMaster sm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        sm.LoadScene(4);
    }
}
