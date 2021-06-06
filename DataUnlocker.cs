using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataUnlocker : MonoBehaviour
{
    protected ChipMaster cm;
    protected LevelData ld;
    protected ScoreMaster scm;
    private void Start()
    {
        cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
        ld = GameObject.Find("_GameMaster").GetComponent<LevelData>();
        scm = GameObject.Find("_ScoreMaster").GetComponent<ScoreMaster>();
        CheckUnlockData();
    }
    public abstract void CheckUnlockData();
    public abstract void UnlockData();
}
