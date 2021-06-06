using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ChipMaster : MonoBehaviour
{
    [SerializeField]
    Chip[] chips;
    [SerializeField]
    WeaponChip[] weaponChips;
    [SerializeField]
    ChipEquipData[] equippedChipData;
    public bool[] unlockedWeapons = new bool[3];
    [HideInInspector]
    public int credits;

    public Sprite nullSprite;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeChipArray();
    }
    void InitializeChipArray()
    {
        for(int i = 0; i < chips.Length; i++)
        {
            chips[i].chipID = i;
        }
    }

    public string GetChipName(int chipIndex)
    {
        return chips[chipIndex].chipClassName;
    }

    public Chip GetChip(int chipIndex)
    {
        return chips[chipIndex];
    }
    public List<Chip> GetUnlockedChips()
    {
        List<Chip> results = new List<Chip>();
        foreach(Chip c in chips)
        {
            if (c.isUnlocked)
            {
                results.Add(c);
            }
        }
        return results;
    }
    public List<Chip> GetUnlockedChips(Chip.chipTypeEnum inputChipType)
    {
        List<Chip> results = new List<Chip>();
        foreach (Chip c in chips)
        {
            if (c.isUnlocked&&c.chipType == inputChipType)
            {
                results.Add(c);
            }
        }
        return results;
    }

    public List<Chip> GetChips(Chip.chipTypeEnum inputChipType)
    {
        List<Chip> results = new List<Chip>();
        foreach (Chip c in chips)
        {
            if (c.chipType == inputChipType)
            {
                results.Add(c);
            }
        }
        return results;
    }
    public WeaponChip GetWeaponChip(string searchClassName)
    {
        for(int i = 0; i < weaponChips.Length; i++)
        {
            if(weaponChips[i].chipClassName.Equals(searchClassName))
            {
                return weaponChips[i];
            }
        }
        return null;
    }
    public WeaponChip GetWeaponChip(int chipIndex)
    {
        for (int i = 0; i < weaponChips.Length; i++)
        {
            if (weaponChips[i].chipID == chipIndex)
            {
                return weaponChips[i];
            }
        }
        return null;
    }
    public void SetChipEquipData(int equipIndex,ChipEquipData chipData)
    {
        equippedChipData[equipIndex] = chipData;
    }
    public ChipEquipData GetChipEquipData(int equipIndex)
    {
        return equippedChipData[equipIndex];
    }
}
