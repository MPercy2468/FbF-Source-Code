using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ChipShopSlot : ChipSlot
{
    [SerializeField]
    protected TextMeshProUGUI chipSlotFireRate;
    [SerializeField]
    protected TextMeshProUGUI chipSlotAmmoCost;
    [SerializeField]
    protected TextMeshProUGUI chipSlotBuyCost;

    ChipShop cs;

    private void Start()
    {
        cs = GameObject.Find("_ChipShop").GetComponent<ChipShop>();
    }
    public void SelectSlotForDisplay()
    {
        cs.DisplayChip(chipIndex);
    }
    public override void SetupChipSlot(int newChipIndex, Chip.chipTypeEnum newChipType)
    {
        base.SetupChipSlot(newChipIndex, newChipType);
        if (newChipType.Equals(Chip.chipTypeEnum.Overclock))
        {
            chipSlotFireRate.text = "Fire Rate: N/A";
            chipSlotAmmoCost.text = "Ammo Cost: N/A";
            ChipMaster cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
            if (cm.GetChip(newChipIndex).isUnlocked)
            {
                chipSlotBuyCost.text = "Purchased";
            }
            else
            {
                chipSlotBuyCost.text = "Buy Cost: " + cm.GetChip(newChipIndex).buyCost.ToString();
            }

            return;
        }
        else
        {
            WeaponChip w = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>().GetWeaponChip(newChipIndex);
            chipSlotFireRate.text = "Fire Rate: "+w.fireRate.ToString();
            chipSlotAmmoCost.text = "Ammo Cost: "+w.ammoCost.ToString();
            if (w.isUnlocked)
            {
                chipSlotBuyCost.text = "Purchased";
            }
            else
            {
                chipSlotBuyCost.text = "Buy Cost: " + w.buyCost;
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //Nothing
    }
}
