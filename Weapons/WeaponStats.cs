using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [HideInInspector]
    public GameMaster gm;
    protected OptionsMaster om;
    protected ChipMaster cm;
    [HideInInspector]
    public AudioController ac;
    protected LineRenderer laser;
    public WeaponChip primaryChip;
    public WeaponChip secondaryChip;
    public Sprite DisplaySprite;
    public Animator WeaponAnimator;
    public SpriteRenderer WeaponRenderer;
    public Transform FirePoint;
    float OriginalPrimaryDamage, OriginalAltDamage;
    [HideInInspector]
    public bool IsActive,IsBeingFired;
    [HideInInspector]
    public bool CanBreakProjectiles;
    public GameObject DashEffect;
    [HideInInspector]
    public bool AltFire;
    public GameMaster.AmmoTypes AmmoType;
    public Chip.chipTypeEnum ChipType;
    [HideInInspector]
    public bool isTempLaserDisable;
    private void Awake()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
        ac = gameObject.GetComponentInChildren<AudioController>();
        om = GameObject.Find("_OptionsMaster").GetComponent<OptionsMaster>();
        cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
        laser = gameObject.GetComponent<LineRenderer>();
    }
    private void Start()
    {
        gm.gc.Player.AltFireToggle.performed += ctx => ToggleAltFire();
        LoadChipEquipData();
    }
    private void Update()
    {
        if (gm.Player.es.IsDead)
        {
            laser.enabled = false;
            return;
        }
        if (IsActive)
        {
            if (!om.isToggleAltFire)
            {
                if (InputHandler.GetAction(gm.gc.Player.AltFireToggle))
                {
                    if(secondaryChip != null)
                    {
                        AltFire = true;
                    }
                }
                else
                {
                    AltFire = false;
                }
            }
            if (!AltFire)
            {
                if (gm.Player.overclock.isBikeRunning)
                {
                    laser.enabled = false;
                    return;
                }
                primaryChip.ChipLogic(this);
                if (isTempLaserDisable)
                {
                    laser.enabled = false;
                }
                else if (om.isWeaponLasersEnabled&&gm.Player.es.GetEntityState() != EntityStats.EntityStates.Melee)
                {
                    primaryChip.CastWeaponLaser(laser);
                    laser.enabled = true;
                }
                else
                {
                    laser.enabled = false;
                }
            }
            else
            {
                if (gm.Player.overclock.isBikeRunning)
                {
                    laser.enabled = false;
                    return;
                }
                secondaryChip.ChipLogic(this);
                if (isTempLaserDisable)
                {
                    laser.enabled = false;
                }
                else if (om.isWeaponLasersEnabled && gm.Player.es.GetEntityState() != EntityStats.EntityStates.Melee)
                {
                    secondaryChip.CastWeaponLaser(laser);
                    laser.enabled = true;
                }
                else
                {
                    laser.enabled = false;
                }
            }
        }
        else
        {
            laser.enabled = false;
        }
        if (primaryChip.fireRateCounter < primaryChip.fireRate)
        {
            primaryChip.fireRateCounter += Time.deltaTime;
        }
        if(secondaryChip != null)
        {
            if (secondaryChip.fireRateCounter < secondaryChip.fireRate)
            {
                secondaryChip.fireRateCounter += Time.deltaTime;
            }
        }

    }
    public void SendAnimatorBool(string Name,bool Value)
    {
        WeaponAnimator.SetBool(Name, Value);
    }
    public void SendAnimatorInt(string Name, int Value)
    {
        WeaponAnimator.SetInteger(Name, Value);
    }
    public void SendAnimatorTrigger(string Name)
    {
        WeaponAnimator.SetTrigger(Name);
    }
    protected void ToggleAltFire()
    {
        if (!om.isToggleAltFire)
        {
            return;
        }
        if (!IsActive||secondaryChip == null||secondaryChip.isMeleeChip)
        {
            return;
        }
        AltFire = !AltFire;
    }
    public void LoadChipEquipData()
    {
        //Primary chip load data
        primaryChip = (WeaponChip)ScriptableObject.CreateInstance(cm.GetChipEquipData((int)ChipType).equippedChips[0].chipClassName);
        primaryChip.CopyWeaponChipCoreValues(cm.GetWeaponChip(cm.GetChipEquipData((int)ChipType).equippedChips[0].chipClassName));
        //Secondary chip load data
        if (cm.GetChipEquipData((int)ChipType).equippedChips[1] != null)
        {
            secondaryChip = (WeaponChip)ScriptableObject.CreateInstance(cm.GetChipEquipData((int)ChipType).equippedChips[1].chipClassName);
            secondaryChip.CopyWeaponChipCoreValues(cm.GetWeaponChip(cm.GetChipEquipData((int)ChipType).equippedChips[1].chipClassName));
        }
        else
        {
            secondaryChip = null;
        }

    }
}
