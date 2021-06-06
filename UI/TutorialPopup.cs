using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPopup : UIPopup
{
    TextMeshProUGUI tutorialTitle;
    Image tutorialImage;
    TextMeshProUGUI tutorialText;
    [SerializeField]
    Sprite image;
    [SerializeField]
    string textTitle;
    [SerializeField]
    [TextArea]
    string textBody;

    float timeToCloseEnable;
    private void Start()
    {
        gm.gc.UI.Click.performed += Context => OnClick();
        popupGroup = gm.um.tutorialGroup;
        tutorialTitle = gm.um.tutorialTitle;
        tutorialImage = gm.um.tutotialImage;
        tutorialText = gm.um.tutorialText;
    }
    public override void StartPopup()
    {
        gm.gc.Player.Disable();
        popupGroup.alpha = 1;
        gm.um.UIState = UIMaster.UIStates.UIPopup;
        Time.timeScale = 0;
        tutorialTitle.text = textTitle;
        tutorialText.text = textBody;
        tutorialImage.sprite = image;
        isStarted = true;
    }

    public override void EndPopup()
    {
        gm.gc.Player.Enable();
        isStarted = false;
        popupGroup.alpha = 0;
        gm.um.UIState = UIMaster.UIStates.PlayerInfo;
        Time.timeScale = 1;
        tutorialTitle.text = "";
        tutorialText.text = "";
        tutorialImage.sprite = null;
    }
    private void Update()
    {
        if (isStarted)
        {
            if(timeToCloseEnable >= 1)
            {
                isFinished = true;
            }
            else
            {
                timeToCloseEnable += Time.unscaledDeltaTime;
            }
        }
    }
    public override void OnClick()
    {
        if (isFinished)
        {
            EndPopup();
            isFinished = false;
        }
    }
}
