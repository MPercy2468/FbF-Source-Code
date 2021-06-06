using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool StartClosed;
    [HideInInspector]
    public bool isCheckpointLoad;
    Animator DoorAnim;
    Collider2D DoorCol;
    [SerializeField]
    SpriteRenderer doorLight;
    [SerializeField]
    AudioController ac;
    private void Awake()
    {
        DoorAnim = gameObject.GetComponent<Animator>();
        DoorCol = gameObject.GetComponent<Collider2D>();
    }
    private void Start()
    {
        if (isCheckpointLoad)
        {
            return;
        }
        if (StartClosed)
        {
            doorLight.color = Color.red;
            DoorAnim.SetTrigger("IsClose");
            DoorCol.enabled = true;
        }
    }
    public void OpenDoor()
    {
        doorLight.color = Color.green;
        ac.PlaySound("DoorOpen", AudioMaster.SoundTypes.SFX, false, false);
        DoorAnim.SetTrigger("IsOpen");
        DoorCol.enabled = false;
    }
    public void CloseDoor()
    {
        doorLight.color = Color.red;
        ac.PlaySound("DoorClose", AudioMaster.SoundTypes.SFX, false, false);
        DoorAnim.SetTrigger("IsClose");
        DoorCol.enabled = true;
    }
}
