using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChipShop : MonoBehaviour
{
    ChipMaster cm;
    List<GameObject> chipShopSlots = new List<GameObject>();
    [HideInInspector]
    public int chipToPurchaseIndex = 0;

    [SerializeField]
    GameObject chipShopSlotPrefab;
    [SerializeField]
    GameObject chipShopSlotContainer;
    [SerializeField]
    TextMeshProUGUI chipName;
    [SerializeField]
    Image chipImage;
    [SerializeField]
    TextMeshProUGUI chipDescription;
    [SerializeField]
    TextMeshProUGUI chipFireRate;
    [SerializeField]
    TextMeshProUGUI chipChipAmmoCost;
    [SerializeField]
    TextMeshProUGUI chipBuyCost;
    private void Start()
    {
        cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
        ChangeShopTab(0);
        DisplayChip(cm.GetChips(Chip.chipTypeEnum.GreenGun)[0].chipID);
    }

    public void PurchaseChip()
    {
        Chip c = cm.GetChip(chipToPurchaseIndex);
        c.isUnlocked = true;
        DisplayChip(chipToPurchaseIndex);
        ChangeShopTab((int)c.chipType);
    }

    public void DisplayChip(int chipIndex)
    {
        Chip c = cm.GetChip(chipIndex);
        if (c.chipType.Equals(Chip.chipTypeEnum.Overclock))
        {
            chipName.text = c.name;
            chipImage.sprite = c.chipSprite;
            chipDescription.text = c.chipDescripton;
            chipFireRate.text = "N/A";
            chipChipAmmoCost.text = "N/A";
            if (c.isUnlocked)
            {
                chipBuyCost.text = "Purchased";
                chipBuyCost.gameObject.GetComponentInParent<Button>().interactable = false;
            }
            else
            {
                chipBuyCost.text = "Buy Cost: " + c.buyCost.ToString();
                chipBuyCost.gameObject.GetComponentInParent<Button>().interactable = true;
            }
            chipToPurchaseIndex = c.chipID;
        }
        else
        {
            WeaponChip w = cm.GetWeaponChip(chipIndex);
            chipName.text = w.name;
            chipImage.sprite = w.chipSprite;
            chipDescription.text = w.chipDescripton;
            chipFireRate.text = w.fireRate.ToString();
            chipChipAmmoCost.text = w.ammoCost.ToString();
            if (w.isUnlocked)
            {
                chipBuyCost.text = "Purchased";
                chipBuyCost.gameObject.GetComponentInParent<Button>().interactable = false;
            }
            else
            {
                chipBuyCost.text = "Buy Cost: " + w.buyCost.ToString();
                chipBuyCost.gameObject.GetComponentInParent<Button>().interactable = true;
            }
            chipToPurchaseIndex = w.chipID;
        }
    }

    public void ChangeShopTab(int chipType)
    {
        LoadChips(chipType);
    }

    void LoadChips(int chipType)
    {
        DestroyChipShopSlots();
        List<Chip> chips = cm.GetChips((Chip.chipTypeEnum)chipType);
        foreach(Chip c in chips)
        {
            GameObject slot = Instantiate(chipShopSlotPrefab, chipShopSlotContainer.transform);
            slot.GetComponent<ChipShopSlot>().SetupChipSlot(c.chipID, c.chipType);
            chipShopSlots.Add(slot);
        }
    }

    void DestroyChipShopSlots()
    {
        for(int i = 0; i < chipShopSlots.Count; i++)
        {
            Destroy(chipShopSlots[i]);
        }
        chipShopSlots.Clear();
    }

    public void BackButtonLogic()
    {
        SceneMaster scm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        if (scm.isChipEquipBack)
        {
            scm.isChipEquipBack = false;
            scm.LoadScene(scm.chipEquipBuildIndex);
        }
        else
        {
            scm.LoadScene(scm.mainMenuBuildIndex);
        }
    }
}
