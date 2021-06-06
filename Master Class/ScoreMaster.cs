using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMaster : MonoBehaviour
{
    public ScoreData scoreData;
    public SectorScoreData[] sectorHighScores;
    private void Awake()
    {
        scoreData = new ScoreData();
        DontDestroyOnLoad(gameObject);
    }
}
