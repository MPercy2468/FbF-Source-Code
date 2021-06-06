using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    List<PreparedSound> PreparedSounds = new List<PreparedSound>();
    [HideInInspector]
    public AudioMaster am;
    [HideInInspector]
    public OptionsMaster om;
    float PreviousSFXVolume, PreviousMSCVolume;
    private void Awake()
    {
        am = GameObject.Find("_AudioMaster").GetComponent<AudioMaster>();
        om = GameObject.Find("_OptionsMaster").GetComponent<OptionsMaster>();
    }
    public void PlaySound(string SoundName,AudioMaster.SoundTypes Channel,bool RandomPitch,bool Loop)
    {
        SoundName = SoundName.ToUpper();
        SoundName = SoundName.Replace(" ", "");
        foreach (PreparedSound s in PreparedSounds)
        {
            if (s.Sound.Name == SoundName)
            {
                s.PlayPreparedSound(RandomPitch,Loop);
                return;
            }
        }
        AudioSource a = this.gameObject.AddComponent<AudioSource>();
        a.rolloffMode = AudioRolloffMode.Linear;
        a.maxDistance = 30;
        a.spatialBlend = 0.8f;
        a.loop = Loop;
        switch (Channel)
        {
            case AudioMaster.SoundTypes.SFX:
                a.volume = om.sfxVolume;
                break;
            case AudioMaster.SoundTypes.MSC:
                a.volume = om.mscVolume;
                break;
        }
        PreparedSound ps = new PreparedSound(am.GetSoundData(SoundName, Channel), a,this);
        PreparedSounds.Add(ps);
        ps.PlayPreparedSound(RandomPitch,Loop);
    }
    public void StopSound(string SoundName)
    {
        SoundName = SoundName.ToUpper();
        SoundName = SoundName.Replace(" ", "");
        foreach (PreparedSound s in PreparedSounds)
        {
            if(s.Sound.Name == SoundName)
            {
                s.StopPreparedSound();
            }
        }
    }
    public void ChangeSFXVolume(float NewVolume)
    {
        om.sfxVolume = NewVolume;
    }
    public void ChangeMSCVolume(float NewVolume)
    {
        om.mscVolume = NewVolume;
    }
    private void Update()
    {
        if(om.sfxVolume != PreviousSFXVolume)
        {
            PreviousSFXVolume = om.sfxVolume;
            foreach(PreparedSound p in PreparedSounds)
            {
                if(p.Sound.Channel == AudioMaster.SoundTypes.SFX)
                {
                    p.UpdateVolume(om.sfxVolume);
                }
            }
        }
        if (om.mscVolume != PreviousMSCVolume)
        {
            PreviousMSCVolume = om.mscVolume;
            foreach (PreparedSound p in PreparedSounds)
            {
                if (p.Sound.Channel == AudioMaster.SoundTypes.MSC)
                {
                    p.UpdateVolume(om.mscVolume);
                }
            }
        }
    }
}
