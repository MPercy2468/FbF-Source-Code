using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SectorSelector : MonoBehaviour
{
    public EventSystem eventSystem;
    [SerializeField]
    GameObject sectorNameText;
    [SerializeField]
    GameObject sectorBestTime;
    [SerializeField]
    GameObject sectorBestScore;

    SceneMaster scm;
    ScoreMaster sm;
    LevelGenerator lg;
    private void Start()
    {
        scm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        sm = GameObject.Find("_ScoreMaster").GetComponent<ScoreMaster>();
        lg = GameObject.Find("_LevelGenerator").GetComponent<LevelGenerator>();
        DisplayScores(0);
    }

    public void LoadSector(int sectorIndex)
    {
        //Changes sector index and generates the floors
        lg.GenerateSector(sectorIndex);
        scm.LoadScene(scm.levelGeneratorSceneBuildIndex);
    }

    public void DisplayScores(int sectorIndex)
    {
        sectorNameText.GetComponent<TextMeshProUGUI>().text = sm.sectorHighScores[sectorIndex].sectorName;
        //Time Conversion
        float levelTime = sm.sectorHighScores[sectorIndex].timeToComplete;
        float levelHours = 0;
        float levelMinutes = 0;
        float levelSeconds = 0;
        string timeText = "Best Time: ";
        string minuteText = "";
        string hourText = "";
        string secondText = "";
        //Get hours
        while (levelTime >= 3600)
        {
            levelTime -= 3600;
            levelHours++;
        }
        if(levelHours < 10)
        {
            hourText = "0" + levelHours;
        }
        else
        {
            hourText = levelHours.ToString();
        }
        //Get minutes
        while (levelTime >= 60)
        {
            levelTime -= 60;
            levelMinutes++;
        }
        if (levelMinutes < 10)
        {
            minuteText = "0" + levelMinutes;
        }
        else
        {
            minuteText = levelMinutes.ToString();
        }
        //Get seconds
        levelSeconds = levelTime;
        levelSeconds = Mathf.RoundToInt(levelSeconds);
        if (levelSeconds < 10)
        {
            secondText = "0" + levelSeconds;
        }
        else
        {
            secondText = levelSeconds.ToString();
        }
        timeText += hourText + ":" + minuteText + ":" + secondText;
        sectorBestTime.GetComponent<TextMeshProUGUI>().text = timeText;
    }

    public void BackButtonLogic()
    {
        scm.LoadScene(scm.chipEquipBuildIndex);
    }
}
