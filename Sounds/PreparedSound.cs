using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class PreparedSound
{
    public Sound Sound;
    AudioSource Source;
    AudioController ac;
    public PreparedSound(Sound InputSound, AudioSource InputSource, AudioController InputAudioController)
    {
        Sound = InputSound;
        Source = InputSource;
        Source.clip = InputSound.Clip;
        Source.playOnAwake = false;
        ac = InputAudioController;
    }
    public void PlayPreparedSound(bool RandomPitch,bool Loop)
    {
        if (Sound.Channel == AudioMaster.SoundTypes.SFX)
        {
            Source.volume = ac.om.sfxVolume;
            if (RandomPitch)
            {
                float NewPitch = Random.Range(0.9f, 1.1f);
                Source.pitch = NewPitch;
            }
            else
            {
                Source.pitch = 1;
            }
            Source.loop = Loop;
            Source.Play();
        }
        else if (Sound.Channel == AudioMaster.SoundTypes.MSC)
        {
            Source.volume = ac.om.mscVolume;
            if (RandomPitch)
            {
                float NewPitch = Random.Range(0.9f, 1.1f);
                Source.pitch = NewPitch;
            }
            else
            {
                Source.pitch = 1;
            }
            Source.loop = Loop;
            Source.Play();
        }
    }
    public void StopPreparedSound()
    {
        Source.Stop();
    }
    public void UpdateVolume(float NewVolume)
    {
        Source.volume = NewVolume;
    }
}
