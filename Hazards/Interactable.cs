using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class Interactable : MonoBehaviour
{
    [Tooltip("If true, can only be interacted with once")]
    public bool SingleInteract;
    [Tooltip("If true, will activate interaction when the trigger is entered")]
    public bool ProximityInteract;
    [SerializeField]
    TextMeshProUGUI popupText;
    [SerializeField]
    UnityEvent Interacted;
    GameMaster gm;
    bool WasInteracted = false;
    bool lockInteraction = false;
    [SerializeField]
    string interactedSound;
    [SerializeField]
    AudioController ac;
    private void Awake()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!WasInteracted)
        {
            if (gm.um.UIState != UIMaster.UIStates.PlayerInfo)
            {
                return;
            }
            if (ProximityInteract)
            {
                DoInteraction();
            }
            else if (InputHandler.GetAction(gm.gc.Player.Interact))
            {
                DoInteraction();
            }
            if (!ProximityInteract&&!lockInteraction)
            {
                if(popupText != null)
                {
                    popupText.enabled = true;
                }
            }
        }
    }
    public void DoInteraction()
    {
        if (!lockInteraction)
        {
            if (SingleInteract)
            {
                lockInteraction = true;
            }
            WasInteracted = true;
            Interacted.Invoke();
            if (ac != null)
            {
                ac.PlaySound(interactedSound, AudioMaster.SoundTypes.SFX, true, false);
            }
        }
    }
    public void LockInteraction()
    {
        lockInteraction = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        WasInteracted = false;
        if(popupText != null)
        {
            popupText.enabled = false;
        }
    }
}
