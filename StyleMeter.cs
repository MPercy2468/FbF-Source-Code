using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleMeter : MonoBehaviour
{
    public enum styleScoreMultiplierEnum
    {
        None,
        EnemyKilled,
        CriticalKilled,
        WeakpointKilled,
        Ricochet,
        NumberOfElements
    }
    public float styleScore;
    float styleScoreOverflow;
    int currentStyleRankIndex;
    public float[] styleScoreReductions = new float[(int)styleScoreMultiplierEnum.NumberOfElements];
    public StyleRank[] styleRanks;
    [SerializeField]
    float styleOverflowRange = 0;
    float styleMeterVisibleCounter;
    bool isStyleMeterOn;
    List<float> styleScores = new List<float>();
    float addStyleScoreCounter;
    float sendStyleScoresCounter;
    float alphaStyle = 1;
    LevelData ld;
    GameMaster gm;
    private void Awake()
    {
        ld = GameObject.Find("_GameMaster").GetComponent<LevelData>();
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
        InitializeStyleRankIndexes();
        InitializeScoreReductions();
    }
    private void Start()
    {
        DisableStyleMeter();
    }
    private void Update()
    {
        CapStyleScore();
        ReduceStyleScore();
        ReduceStyleScoreReductions();
        SetStyleRankIndex();
        AddStyleScoreToList();
        CheckStyleMeterVisible();
        FadeStyleMeter();
    }
    public void IncreaseStyleScore(float score,styleScoreMultiplierEnum multi)
    {
        if(score <= 0.5f)
        {
            return;
        }
        EnableStyleMeter();
        float scoreToAdd = score * styleScoreReductions[(int)multi];
        styleScoreReductions[(int)multi] -= 0.1f;
        if(styleScoreReductions[(int)multi] < 0)
        {
            styleScoreReductions[(int)multi] = 0;
        }
        styleScore += scoreToAdd;
    }
    public void DecreaseStyleScore(float score)
    {
        styleScore -= score;
    }
    void InitializeScoreReductions()
    {
        for(int i = 0; i < styleScoreReductions.Length; i++)
        {
            styleScoreReductions[i] = 1;
        }
    }
    void InitializeStyleRankIndexes()
    {
        for(int i = 0; i < styleRanks.Length; i++)
        {
            styleRanks[i].rankIndex = i;
        }
    }
    void ReduceStyleScoreReductions()
    {
        for (int i = 0; i < styleScoreReductions.Length; i++)
        {
            if(styleScoreReductions[i] > 1)
            {
                styleScoreReductions[i] = 1;
            }
            else
            {
                styleScoreReductions[i] += 0.05f * Time.deltaTime;
            }
        }
    }

    void ReduceStyleScore()
    {
        if(styleScore < 0)
        {
            styleScore = 0;
            return;
        }
        styleScore -= styleRanks[currentStyleRankIndex].rankScoreReductionRate*Time.deltaTime;
    }
    void CapStyleScore()
    {
        if (styleScore > styleRanks[styleRanks.Length - 1].rankScoreMaximum + styleOverflowRange)
        {
            styleScoreOverflow += styleScore - styleRanks[styleRanks.Length - 1].rankScoreMaximum + styleOverflowRange;
            styleScore = styleRanks[styleRanks.Length - 1].rankScoreMaximum + styleOverflowRange;
        }
    }
    void SetStyleRankIndex()
    {
        foreach(StyleRank s in styleRanks)
        {
            if(styleScore < s.rankScoreMaximum)
            {
                currentStyleRankIndex = s.rankIndex;
                return;
            }
        }
    }
    public float GetStyleScoreRatio()
    {
        float mod = 0;
        if(currentStyleRankIndex-1 >= 0)
        {
            mod = styleRanks[currentStyleRankIndex - 1].rankScoreMaximum;
        }
        float denom = styleRanks[currentStyleRankIndex].rankScoreMaximum-mod;
        float nume = styleScore - mod;
        return nume / denom;
    }

    public Sprite GetStyleRankSprite()
    {
        return styleRanks[currentStyleRankIndex].rankSprite;
    }

    void EnableStyleMeter()
    {
        isStyleMeterOn = true;
        gm.um.GetStyleMeterCanvasGroup().alpha = 1;
        styleMeterVisibleCounter = 0;
    }
    void DisableStyleMeter()
    {
        isStyleMeterOn = false;
        SendAverageStyleScore();
    }
    void SendAverageStyleScore()
    {
        float avg = 0;
        for (int i = 0; i < styleScores.Count;i++)
        {
            avg += styleScores[i];
        }
        avg = avg / styleScores.Count;
        styleScores.Clear();
        ld.scoreData.averageStyleScores.Add(avg);
    }
    void AddStyleScoreToList()
    {
        if (!isStyleMeterOn)
        {
            return;
        }
        if (addStyleScoreCounter >= 3)
        {
            addStyleScoreCounter = 0;
            styleScores.Add(styleScore);
        }
        else
        {
            addStyleScoreCounter += Time.deltaTime;
        }
    }
    void CheckStyleMeterVisible()
    {
        if (!isStyleMeterOn)
        {
            return;
        }
        if(styleMeterVisibleCounter >= 5)
        {
            styleMeterVisibleCounter = 0;
            DisableStyleMeter();
        }
        else
        {
            styleMeterVisibleCounter += Time.deltaTime;
        }
    }
    void FadeStyleMeter()
    {
        if (isStyleMeterOn)
        {
            return;
        }
        alphaStyle = Mathf.Lerp(alphaStyle, 0,Time.deltaTime);
        gm.um.GetStyleMeterCanvasGroup().alpha = alphaStyle;
    }
}
