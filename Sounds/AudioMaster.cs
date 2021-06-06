using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour
{
    //Arrays used to set up sound objects in the unity engine
    public Sound[] SFXToInitialize, MSCToInitialize;
    //Dictionaries that will recieve data transferred from the arrays
    Dictionary<string, Sound> SFXData = new Dictionary<string, Sound>(), MSCData = new Dictionary<string, Sound>();
    public enum SoundTypes
    {
      SFX,
      MSC
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InitializeSounds();
    }
    void InitializeSounds()
    {
        foreach (Sound s in SFXToInitialize)
        {
            s.Name = s.Name.ToUpper();
            s.Name = s.Name.Replace(" ", "");
            SFXData.Add(s.Name, s);
        }
        foreach (Sound s in MSCToInitialize)
        {
            MSCData.Add(s.Name, s);
        }
    }
    public Sound GetSoundData(string Search, SoundTypes Channel)
    {
        if (Channel == SoundTypes.SFX)
        {
            if (SFXData.ContainsKey(Search))
            {
                Sound Result;
                SFXData.TryGetValue(Search, out Result);
                return Result;
            }
            else
            {
                return null;
            }
        }
        else if (Channel == SoundTypes.MSC)
        {
            if (MSCData.ContainsKey(Search))
            {
                Sound Result;
                MSCData.TryGetValue(Search, out Result);
                return Result;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
