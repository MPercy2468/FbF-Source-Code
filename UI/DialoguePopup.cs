using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialoguePopup : UIPopup
{
    [HideInInspector]
    public TextMeshProUGUI popupText;
    [HideInInspector]
    public Image popupImage;
    [HideInInspector]
    public TextMeshProUGUI popupName;
    [SerializeField]
    List<Dialogue> dialogues = new List<Dialogue>();
    int dialogueIndex = 0;
    [HideInInspector]
    public bool isDisplaying, isWaitForPrompt, canSpeedUp;
    private void Start()
    {
        gm.gc.UI.Click.performed += Context => OnClick();
        popupGroup = gm.um.GetDialogueCanvasGroup();
        popupText = gm.um.GetDialogueText();
        popupName = gm.um.GetDialogueNameText();
        popupImage = gm.um.GetDialogueImage();
    }
    public override void StartPopup()
    {
        gm.gc.Player.Disable();
        dialogueIndex = 0;
        popupGroup.alpha = 1;
        gm.um.UIState = UIMaster.UIStates.UIPopup;
        Time.timeScale = 0;
        StartDialogue();
    }

    public override void EndPopup()
    {
        gm.gc.Player.Enable();
        isStarted = false;
        isDisplaying = false;
        popupGroup.alpha = 0;
        gm.um.UIState = UIMaster.UIStates.PlayerInfo;
        Time.timeScale = 1;
    }
    void StartDialogue()
    {
        isStarted = true;
    }
    private void Update()
    {
        if (isStarted)
        {
            if (isWaitForPrompt)
            {
                return;
            }
            if (!isDisplaying)
            {
                isDisplaying = true;
                StartCoroutine(dialogues[dialogueIndex].DisplayText(this));
            }
        }
    }
    public override void OnClick()
    {
        if (!isStarted)
        {
            return;
        }
        if (isWaitForPrompt)
        {
            NextDialogue();
        }
        else if (canSpeedUp)
        {
            canSpeedUp = false;
            dialogues[dialogueIndex].typeDelay = 0;
        }
    }
    public void NextDialogue()
    {
        dialogueIndex++;
        isWaitForPrompt = false;
        if (dialogueIndex == dialogues.Count)
        {
            EndPopup();
            return;
        }
        isDisplaying = false;
    }
}
