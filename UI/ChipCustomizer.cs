using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChipCustomizer : MonoBehaviour
{

    //UI Variables
    public EventSystem eventSystem;
    [SerializeField]
    GameObject[] weaponSlots;
    [SerializeField]
    GameObject chipInventorySlotPrefab;
    [SerializeField]
    GameObject chipInventoryParent;
    [SerializeField]
    GameObject oldChipInventoryDescription;
    [SerializeField]
    GameObject oldChipInventoryName;
    [SerializeField]
    GameObject oldChipInventoryImage;
    [SerializeField]
    GameObject newChipInventoryName;
    [SerializeField]
    GameObject newChipInventoryImage;
    [SerializeField]
    GameObject newChipInventoryDescription;
    [SerializeField]
    CanvasGroup chipInventoryCanvasGroup;

    //Temp variables
    int chipEquipIndex;

    [SerializeField]
    AudioController ac;
    ChipMaster cm;
    Chip selectedChip;
    List<ChipSlot> chipInventorySlots = new List<ChipSlot>();

    private void Awake()
    {
        cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
    }

    private void Start()
    {
        SetUnlockedWeaponSlots();
    }
    void SetUnlockedWeaponSlots()
    {
        for(int i = 0; i < weaponSlots.Length; i++)
        {
            if(i == 3)
            {
                weaponSlots[i].SetActive(true);
            }
            else if (!cm.unlockedWeapons[i])
            {
                weaponSlots[i].SetActive(false);
            }
            else
            {
                weaponSlots[i].SetActive(true);
            }
        }
    }
    void CreateChipInventoryButtons(Chip.chipTypeEnum chipType)
    {
        //Chip Inventory
        List<Chip> unlockedChips = cm.GetUnlockedChips(chipType);
        foreach(Chip c in unlockedChips)
        {
            GameObject spawnedSlot = Instantiate(chipInventorySlotPrefab,chipInventoryParent.transform);
            ChipSlot slot = spawnedSlot.GetComponent<ChipSlot>();
            slot.SetupChipSlot(c.chipID,chipType);
            if (!chipInventorySlots.Contains(slot))
            {
                chipInventorySlots.Add(slot);
            }
            else
            {
                Destroy(spawnedSlot.gameObject);
            }
        }
        if(chipInventorySlots.Count > 0)
        {
            SetSelectedChip(chipInventorySlots[0].chipIndex);
            eventSystem.SetSelectedGameObject(chipInventorySlots[0].gameObject);
        }
    }

    void DestroyChipInventorySlots()
    {
        for (int i = 0; i < chipInventorySlots.Count; i++)
        {
            Destroy(chipInventorySlots[i].gameObject);
        }
        chipInventorySlots.Clear();
    }
    void OpenChipInventory()
    {
        chipInventoryCanvasGroup.alpha = 1;
        chipInventoryCanvasGroup.interactable = true;
        chipInventoryCanvasGroup.blocksRaycasts = true;
    }
    void CloseChipInventory()
    {
        chipInventoryCanvasGroup.alpha = 0;
        chipInventoryCanvasGroup.interactable = false;
        chipInventoryCanvasGroup.blocksRaycasts = false;
    }
    void ResetWeaponSlots()
    {
        for(int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].SetActive(false);
        }
        SetUnlockedWeaponSlots();
    }
    public void StartChipEquip(int chipTypeIndex)
    {
        Chip.chipTypeEnum c = (Chip.chipTypeEnum)chipTypeIndex;
        OpenChipInventory();
        CreateChipInventoryButtons(c);
    }
    public void AssignChipEquipIndex(int index)
    {
        chipEquipIndex = index;
    }
    public void ChipEquip(int chipDataIndex, Chip.chipTypeEnum chipType)
    {
        ac.PlaySound("UIEquip", AudioMaster.SoundTypes.SFX, false, false);
        ChipEquipData cd = cm.GetChipEquipData((int)chipType);
        cd.equippedChips[chipEquipIndex] = cm.GetChip(chipDataIndex);
        cm.SetChipEquipData((int)chipType, cd);
        DestroyChipInventorySlots();
        CloseChipInventory();
        ResetWeaponSlots();
    }

    public void SetSelectedChip(int index)
    {
        selectedChip = cm.GetChip(index);
        newChipInventoryImage.GetComponent<Image>().sprite = selectedChip.chipSprite;
        newChipInventoryName.GetComponent<TextMeshProUGUI>().text = selectedChip.name;
        newChipInventoryDescription.GetComponent<TextMeshProUGUI>().text = selectedChip.chipDescripton;
    }
    public void BackButtonLogic()
    {
        SceneMaster scm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        scm.LoadScene(scm.mainMenuBuildIndex);
    }
    public void ShopButtonLogic()
    {
        SceneMaster scm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        scm.isChipEquipBack = true;
        scm.LoadScene(scm.chipShopBuildIndex);
    }
    public void ContinueButtonLogic()
    {
        SceneMaster scm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        scm.LoadScene(scm.sectorSelectBuildIndex);
    }
}
