using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameMaster : MonoBehaviour
{
    public Camera MainCam;
    public GameControls gc;
    public CameraController cc;
    public UIMaster um;
    public OptionsMaster om;
    [HideInInspector]
    public ChipMaster cm;
    [HideInInspector]
    public LevelData ld;
    [HideInInspector]
    public LevelGenerator lg;
    //boolean for finding level generator
    [SerializeField]
    bool isFindLevelGen;
    public WeaponStats ActivePlayerWeapon;
    public WeaponStats[] PlayerWeapons;
    public Player Player;
    public enum AmmoTypes
    {
        Ballistic,Energy,Special,None
    }
    public int BallistcAmmoMax, EnergyAmmoMax, SpecialAmmoMax;
    int BallisticAmmo, EnergyAmmo, SpecialAmmo;
    public int RedGrenadesMax, BlueGrenadesMax, GreenGrenadesMax;
    int RedGrenades, BlueGrenades, GreenGrenades;
    //Volume references;
    [SerializeField]
    Volume SceneVolume;
    public VolumeProfile DashProfile;
    public VolumeProfile OverclockProfile;
    public VolumeProfile NeutralProfile;
    //Finisher References
    public GameObject FinisherHealthPickup,FinisherBallisticAmmoPickup,FinisherEnergyAmmoPickup,FinisherSpecialAmmoPickup;
    public Material DefaultMaterial, FinisherMatirial,StunMaterial;

    //Gib and Gore References
    public GameObject[] OrganicGore, CyberneticGore;
    public GameObject gibTemplatePrefab;

    //Navmesh references
    public NavMeshSurface2d navMeshSurface;

    //Melee effect matarials
    public Material defaultWeaponMaterial, swordMaterial, spearMaterial, axeMaterial;


    SceneMaster sm;
    ScoreMaster scm;

    private void Awake()
    {
        sm = GameObject.Find("_SceneMaster").GetComponent<SceneMaster>();
        scm = GameObject.Find("_ScoreMaster").GetComponent<ScoreMaster>();
        ld = gameObject.GetComponent<LevelData>();
        om = GameObject.Find("_OptionsMaster").GetComponent<OptionsMaster>();
        gc = om.gc;
        cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
        if (isFindLevelGen)
        {
            lg = GameObject.Find("_LevelGenerator").GetComponent<LevelGenerator>();
        }
        AddAmmo(BallistcAmmoMax / 2, AmmoTypes.Ballistic);
        AddAmmo(EnergyAmmoMax / 2, AmmoTypes.Energy);
        AddAmmo(SpecialAmmoMax / 2, AmmoTypes.Special);
        
        string insert = "trains";
        string remove = "";
        string output = "I like [REDACTED]";
        bool isRead = false;
        foreach(char c in output)
        {
            if (c.Equals('['))
            {
                isRead = true;
                remove += c;
            }
            else if (c.Equals(']'))
            {
                isRead = false;
                remove += c;
                break;
            }
            else if (isRead)
            {
                remove += c;
            }
        }
        output = output.Replace(remove, insert);
    }
    private void Start()
    {
        gc.Player.WeaponKeys.performed += Context => SwitchActiveWeapon(Context.action);
        //InputUser.onChange += onInputDeviceChange;
        ActivePlayerWeapon = PlayerWeapons[0];
        ActivePlayerWeapon.IsActive = true;
        ActivePlayerWeapon.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    private void Update()
    {      
        CapAmmo();
    }

    void CapAmmo()
    {

        if(BallisticAmmo < 0)
        {
            BallisticAmmo = 0;
        }
        if(BallisticAmmo > BallistcAmmoMax)
        {
            BallisticAmmo = BallistcAmmoMax;
        }
        if (EnergyAmmo < 0)
        {
            EnergyAmmo = 0;
        }
        if (EnergyAmmo > EnergyAmmoMax)
        {
            EnergyAmmo = EnergyAmmoMax;
        }
        if (SpecialAmmo < 0)
        {
            SpecialAmmo = 0;
        }
        if (SpecialAmmo > SpecialAmmoMax)
        {
            SpecialAmmo = SpecialAmmoMax;
        }
    }
    int PreviousWeaponIndex = 0;
    public void SwitchActiveWeapon(InputAction SwitchAction)
    {
        EquipWeapon(SwitchAction.GetBindingIndexForControl(SwitchAction.activeControl));
    }
    public void EquipWeapon(int Index)
    {
        try
        {
            if(ActivePlayerWeapon == PlayerWeapons[Index])
            {
                return;
            }
            else if (cm.unlockedWeapons[Index])
            {
                Player.es.ac.PlaySound("WeaponSwap", AudioMaster.SoundTypes.SFX, false, false);
                ActivePlayerWeapon.SendAnimatorInt("IsBlade", 0);
                ActivePlayerWeapon.SendAnimatorInt("IsSpear", 0);
                ActivePlayerWeapon.SendAnimatorInt("IsAxe", 0);
                ActivePlayerWeapon.isTempLaserDisable = false;
                ActivePlayerWeapon.IsActive = false;
                ActivePlayerWeapon.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                ActivePlayerWeapon = PlayerWeapons[Index];
                PreviousWeaponIndex = Index;
                ActivePlayerWeapon.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
                ActivePlayerWeapon.IsActive = true;
            }

        }
        catch (Exception e)
        {
            Debug.Log("No weapon registered here");
            ActivePlayerWeapon = PlayerWeapons[PreviousWeaponIndex];
            ActivePlayerWeapon.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
            ActivePlayerWeapon.IsActive = true;
        }
    }

    public void LoadWeaponChips()
    {
        foreach(WeaponStats s in PlayerWeapons)
        {
            s.LoadChipEquipData();
        }
    }
    public void RemoveAmmo(int Ammount,AmmoTypes AmmoType)
    {
        if(AmmoType == AmmoTypes.Ballistic)
        {
            BallisticAmmo -= Ammount;
        }
        else if(AmmoType == AmmoTypes.Energy)
        {
            EnergyAmmo -= Ammount;
        }
        else if(AmmoType == AmmoTypes.Special)
        {
            SpecialAmmo -= Ammount;
        }
    }

    public void AddAmmo(int Ammount, AmmoTypes AmmoType)
    {
        if (AmmoType == AmmoTypes.Ballistic)
        {
            BallisticAmmo += Ammount;
        }
        else if (AmmoType == AmmoTypes.Energy)
        {
            EnergyAmmo += Ammount;
        }
        else if (AmmoType == AmmoTypes.Special)
        {
            SpecialAmmo += Ammount;
        }
    }

    public int CheckAmmo(int Ammount,AmmoTypes AmmoType)
    {
        int Check = 0;
        if (AmmoType == AmmoTypes.Ballistic)
        {
            Check = BallisticAmmo;
        }
        else if (AmmoType == AmmoTypes.Energy)
        {
            Check = EnergyAmmo;
        }
        else if (AmmoType == AmmoTypes.Special)
        {
            Check = SpecialAmmo;
        }
        Check -= Ammount;
        return Check;
    }

    public int GetAmmo(AmmoTypes AmmoType)
    {
        if (AmmoType == AmmoTypes.Ballistic)
        {
            return BallisticAmmo;
        }
        else if (AmmoType == AmmoTypes.Energy)
        {
            return EnergyAmmo;
        }
        else if (AmmoType == AmmoTypes.Special)
        {
            return SpecialAmmo;
        }
        return 0;
    }
    public void ChangeVolume(VolumeProfile Profile)
    {
        SceneVolume.profile = Profile;
    }
    void onInputDeviceChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.ControlSchemeChanged)
        {
            InputHandler.UpdateCurrentDevice(user.controlScheme.Value.name);
        }
    }


    public void DebugDrawBox(Vector2 point, Vector2 size, float angle, Color color, float duration)
    {
        var orientation = Quaternion.Euler(0, 0, angle);
        //Basis vectors, half the size in each direction from the center.
        Vector2 right = orientation * Vector2.right * size.x / 2f;
        Vector2 up = orientation * Vector2.up * size.y / 2f;
        //Four box corners.
        var topLeft = point + up - right;
        var topRight = point + up + right;
        var bottomRight = point - up + right;
        var bottomLeft = point - up - right;
        //Now we've reduced the problem to drawing lines.
        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration); 
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
    public void RebuildNavmesh()
    {
        Debug.Log("Rebuilding navmesh");
        navMeshSurface.BuildNavMesh();
    }
    public void LoadDeathScene()
    {
        scm.scoreData = ld.scoreData;
        sm.LoadScene(sm.deathSceneBuildIndex);
    }
}
