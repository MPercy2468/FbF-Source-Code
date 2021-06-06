using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMaster : MonoBehaviour
{
    public bool isWeaponLasersEnabled;
    public bool isToggleAltFire;
    public float sfxVolume;
    public float mscVolume;
    public float mouseSensitivity;

    public GameControls gc;

    [HideInInspector]
    public AudioController ac;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ac = GetComponent<AudioController>();
        gc = new GameControls();
    }

    private void OnEnable()
    {
        gc.Enable();
    }
    private void OnDisable()
    {
        gc.Disable();
    }
}
