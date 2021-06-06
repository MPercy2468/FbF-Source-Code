using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    Collider2D DoorCollider;
    [SerializeField]
    bool isStartClose;
    [SerializeField]
    GameObject lightObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        UseElevator();
    }
    private void Start()
    {
        if (isStartClose)
        {
            CloseElevator();
        }
    }
    public virtual void OpenElevator()
    {
        gameObject.GetComponent<Animator>().SetTrigger("IsOpen");
        DoorCollider.enabled = false;
        lightObject.SetActive(true);
    }

    public virtual void CloseElevator()
    {
        gameObject.GetComponent<Animator>().SetTrigger("IsClose");
        DoorCollider.enabled = true;
        lightObject.SetActive(false);
    }
    void UseElevator()
    {
        Time.timeScale = 1f;
        CloseElevator();
        Invoke("LoadNextScene",2f);
    }
    public virtual void LoadNextScene()
    {
        //Send level score to score master
        ScoreMaster scm = GameObject.Find("_ScoreMaster").GetComponent<ScoreMaster>();
        LevelData ld = GameObject.Find("_GameMaster").GetComponent<LevelData>();
        scm.scoreData = ld.scoreData;
        //Iterate level generator floor index
        LevelGenerator lg = GameObject.Find("_LevelGenerator").GetComponent<LevelGenerator>();
        lg.floorIndex++;
        lg.hasLoadRoomOrder = true;
        //Set build index of level generator
        SceneMaster sm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        sm.LoadScene(sm.levelGeneratorSceneBuildIndex);
    }
}
