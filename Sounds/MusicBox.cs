using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBox : MonoBehaviour
{
    AudioController ac;
    OptionsMaster om;
    AudioSource Source;
    float trackCounter, trackLength;
    public Sound[] Music;
    private void Start()
    {
        ac = gameObject.GetComponent<AudioController>();
        om = GameObject.Find("_OptionsMaster").GetComponent<OptionsMaster>();
        Source = gameObject.AddComponent<AudioSource>();
        Source.clip = Music[0].Clip;
        Source.loop = true;
        Source.Play();
    }
    private void Update()
    {
        Source.volume = om.mscVolume;
    }
}
