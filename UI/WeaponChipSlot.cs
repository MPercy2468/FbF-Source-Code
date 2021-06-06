using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChipSlot : MonoBehaviour
{
    [SerializeField]
    Image slotImage;
    [SerializeField]
    int chipEquipIndex;
    [SerializeField]
    int chipTypeIndex;

    ChipMaster cm;

    private void Start()
    {
        cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
        if(cm.GetChipEquipData(chipTypeIndex).equippedChips[chipEquipIndex] != null)
        {
            slotImage.sprite = cm.GetChipEquipData(chipTypeIndex).equippedChips[chipEquipIndex].chipSprite;
        }
        else
        {
            slotImage.sprite = cm.nullSprite;
        }
    }
    
    private void OnEnable()
    {
        if (cm)
        {
            if (cm.GetChipEquipData(chipTypeIndex).equippedChips[chipEquipIndex] != null)
            {
                slotImage.sprite = cm.GetChipEquipData(chipTypeIndex).equippedChips[chipEquipIndex].chipSprite;
            }
            else
            {
                slotImage.sprite = cm.nullSprite;
            }
        }
    }
}
