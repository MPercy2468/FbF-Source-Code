using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [SerializeField]
    string speakerName;
    [SerializeField]
    Sprite speakerImage;
    [SerializeField]
    [TextArea]
    string text;
    public float typeDelay;
    float originalDelay;
    public IEnumerator DisplayText(DialoguePopup popup)
    {
        popup.popupImage.sprite = speakerImage;
        popup.popupName.text = speakerName;
        originalDelay = typeDelay;
        popup.popupText.text = "";
        foreach(char c in text)
        {
            popup.canSpeedUp = true;
            popup.popupText.text += c;
            Debug.Log(typeDelay);
            yield return new WaitForSecondsRealtime(typeDelay);
        }
        popup.isWaitForPrompt = true;
        typeDelay = originalDelay;
        yield return null;
    }
}
