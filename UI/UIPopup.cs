using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIPopup : MonoBehaviour
{
    protected GameMaster gm;
    protected CanvasGroup popupGroup;
    protected bool isStarted = false;
    protected bool isFinished = false;
    private void Awake()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
    }
    public abstract void StartPopup();
    public abstract void EndPopup();
    public abstract void OnClick();
}
