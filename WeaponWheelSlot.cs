using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelSlot : MonoBehaviour
{
    [SerializeField]
    int WeaponIndex;
    [SerializeField]
    float StartSelectionAngle, StopSelectionAngle;
    public GameMaster.AmmoTypes WeaponAmmoType;
    public Sprite[] WeaponWheelSlotSprites;
    [SerializeField]
    Image BackgroundImage;
    public bool CheckSelectionAngle(float Angle)
    {      
        if(Angle > StartSelectionAngle && Angle < StopSelectionAngle)
        {
            if(WeaponAmmoType == GameMaster.AmmoTypes.Ballistic)
            {
                BackgroundImage.sprite = WeaponWheelSlotSprites[1];
            }
            else if(WeaponAmmoType == GameMaster.AmmoTypes.Energy)
            {
                BackgroundImage.sprite = WeaponWheelSlotSprites[2];
            }
            else if(WeaponAmmoType == GameMaster.AmmoTypes.Special)
            {
                BackgroundImage.sprite = WeaponWheelSlotSprites[3];
            }
            return true;
        }
        else
        {
            Debug.Log(Angle);
        }
        BackgroundImage.sprite = WeaponWheelSlotSprites[0];
        return false;
    }

    public int GetWeaponIndex()
    {
        return WeaponIndex;
    }
}
