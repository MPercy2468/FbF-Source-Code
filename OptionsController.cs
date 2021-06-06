using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsController : MonoBehaviour
{
    OptionsMaster om;

    private void Awake()
    {
        om = GameObject.Find("_OptionsMaster").GetComponent<OptionsMaster>();
        om.ac = GetComponent<AudioController>();
    }
    public void EnableWeaponLaser(bool input)
    {
        om.isWeaponLasersEnabled = input;
        if (!input)
        {
            om.ac.PlaySound("UIDecline",AudioMaster.SoundTypes.SFX,false,false);
        }
        else
        {
            om.ac.PlaySound("UIAccept", AudioMaster.SoundTypes.SFX, false, false);
        }
    }
    public void ToggleAltFire(bool input)
    {
        om.isToggleAltFire = input;
        if (!input)
        {
            om.ac.PlaySound("UIDecline", AudioMaster.SoundTypes.SFX, false, false);
        }
        else
        {
            om.ac.PlaySound("UIAccept", AudioMaster.SoundTypes.SFX, false, false);
        }
    }
}
