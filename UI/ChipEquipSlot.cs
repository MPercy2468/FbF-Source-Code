using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipEquipSlot : MonoBehaviour
{
    [SerializeField]
    Image slotImage;
    Chip equippedChip;
    public void SetupChipEquipSlot(Chip c)
    {
        equippedChip = c;
        slotImage.sprite = equippedChip.chipSprite;
    }
}
