using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChipSlot : MonoBehaviour, IPointerEnterHandler
{
    public int chipIndex;
    [HideInInspector]
    public Chip.chipTypeEnum chipType;
    [SerializeField]
    protected Image chipSlotImage;
    [SerializeField]
    protected TextMeshProUGUI chipSlotName;
    ChipCustomizer cc;

    private void Start()
    {
        cc = GameObject.Find("_ChipCustomizer").GetComponent<ChipCustomizer>();
    }
    public virtual void SetupChipSlot(int newChipIndex,Chip.chipTypeEnum newChipType)
    {
        chipIndex = newChipIndex;
        chipType = newChipType;
        Chip c = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>().GetChip(chipIndex);
        chipSlotName.text = c.name;
        chipSlotImage.sprite = c.chipSprite;
    }
    public virtual void SlotPressed()
    {
        cc.ChipEquip(chipIndex,chipType);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        cc.eventSystem.SetSelectedGameObject(gameObject);
        cc.SetSelectedChip(chipIndex);
    }
}
