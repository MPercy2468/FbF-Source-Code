using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overclock : MonoBehaviour
{
    public Player player;
    [HideInInspector]
    public bool isOverclockActive = false;
    [SerializeField]
    float maxOverclockCharge;
    float currentOverclockCharge;
    public OverclockProfile overclockProfile = new OverclockProfile();
    [HideInInspector]
    public bool isBulletBuster = false;
    //Bike variables
    public Damage bikeDamage;
    [HideInInspector]
    public bool isBike = false;
    [HideInInspector]
    public bool isBikeRunning = false;
    List<EntityStats> bikeAlreadyHits = new List<EntityStats>();
    //Plasma surge variables
    [HideInInspector]
    public bool isPlasmaSurge = false;
    public Damage plasmaSurgeDamage;
    //Bubble shield variables
    [SerializeField]
    GameObject overclockBubbleShield;
    [HideInInspector]
    public bool isBubbleShield = false;
    ChipMaster cm;
    [SerializeField]
    bool isInfOverclock;
    private void Start()
    {
        if(isInfOverclock){
            currentOverclockCharge = maxOverclockCharge;
        }
        currentOverclockCharge = maxOverclockCharge;
        player.es.gm.gc.Player.Overclock.performed += Context => ToggleOverclock();
        cm = GameObject.Find("_ChipMaster").GetComponent<ChipMaster>();
    }
    public void ToggleOverclock()
    {
        LoadEquippedChips();
        isOverclockActive = !isOverclockActive;
        if (isOverclockActive)
        {
            if (currentOverclockCharge <= 86)
            {
                isOverclockActive = false;
                return;
            }
            player.es.ac.PlaySound("OverclockOn", AudioMaster.SoundTypes.SFX, false, false);
            player.es.ac.PlaySound("OverclockAmbient", AudioMaster.SoundTypes.SFX, false, true);
            player.es.gm.ChangeVolume(player.es.gm.OverclockProfile);
        }
        else
        {
            player.es.ac.PlaySound("OverclockOff", AudioMaster.SoundTypes.SFX, false, false);
            player.es.ac.StopSound("OverclockAmbient");
            player.es.gm.ChangeVolume(player.es.gm.NeutralProfile);
            StopBikeLogic();
        }
    }
    
    public void ModifyOverclockCharge(float input)
    {
        if(isInfOverclock){
            return;
        }
        currentOverclockCharge += input;
    }
    public float GetCurrentOverclockCharge()
    {
        return currentOverclockCharge;
    }
    public float GetMaxOverclockCharge()
    {
        return maxOverclockCharge;
    }
    public bool GetIsOverClockActive()
    {
        return isOverclockActive;
    }
    void LoadEquippedChips()
    {
        for(int i = 0; i < 4; i++)
        {
            if(cm.GetChipEquipData((int)Chip.chipTypeEnum.Overclock).equippedChips[i] != null)
            {
                overclockProfile.overclockChips[i] = 
                    (OverclockChip)ScriptableObject.CreateInstance(cm.GetChipEquipData((int)Chip.chipTypeEnum.Overclock).equippedChips[i].chipClassName);
            }
        }
    }
    void CapOverclockValue()
    {
        if(currentOverclockCharge > maxOverclockCharge)
        {
            currentOverclockCharge = maxOverclockCharge;
        }
    }
    private void Update()
    {
        CapOverclockValue();
        if (!isOverclockActive)
        {
            isPlasmaSurge = false;
            isBubbleShield = false;
            isBulletBuster = false;
            isBike = false;

            overclockBubbleShield.SetActive(false);
            return;
        }
        if(currentOverclockCharge <= 0)
        {
            isOverclockActive = false;
            StopBikeLogic();
            player.es.ac.PlaySound("OverclockOff", AudioMaster.SoundTypes.SFX, false, false);
            player.es.ac.StopSound("OverclockAmbient");
            player.es.gm.ChangeVolume(player.es.gm.NeutralProfile);
            return;
        }
        if (isBubbleShield)
        {
            overclockBubbleShield.SetActive(true);
        }
        if (isBike)
        {
            if (InputHandler.GetAction(player.es.gm.gc.Player.Dash))
            {
                RunBikeLogic();
            }
            else
            {
                StopBikeLogic();
            }
        }
        overclockProfile.RunOverclockEffects(this);
    }

    void RunBikeLogic()
    {
        //Sets bike running to true, used to disable weapons while bike is being ridden
        isBikeRunning = true;
        //Sets animation, entity state, and velocity
        player.es.gm.ActivePlayerWeapon.SendAnimatorBool("IsBike", true);
        player.es.SetEntityState(EntityStats.EntityStates.Dashing);
        player.es.rb.velocity = transform.up * player.DashSpeed;
        //Hitbox collision logic
        //Casts a box over the bike, applies damage to enemies within
        Collider2D[] Hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1.5f, 4), transform.rotation.eulerAngles.z, LayerMask.GetMask("Enemies"));
        List<EntityStats> ToKeep = new List<EntityStats>();
        List<EntityStats> ToRemove = new List<EntityStats>();
        foreach (Collider2D c in Hits)
        {
            if (bikeAlreadyHits.Contains(c.GetComponentInParent<EntityStats>()))
            {
                ToKeep.Add(c.GetComponentInParent<EntityStats>());
            }
        }
        for (int x = 0; x < bikeAlreadyHits.Count; x++)
        {
            if (!ToKeep.Contains(bikeAlreadyHits[x]))
            {
                ToRemove.Add(bikeAlreadyHits[x]);
            }
        }
        for (int x = 0; x < ToRemove.Count; x++)
        {
            bikeAlreadyHits.Remove(ToRemove[x]);
        }
        ToRemove.Clear();
        ToKeep.Clear();
        foreach (Collider2D c in Hits)
        {
            EntityStats e = c.GetComponentInParent<EntityStats>();
            if (!bikeAlreadyHits.Contains(e))
            {
                bikeDamage.Direction = transform.up;
                e.TakeDamage(bikeDamage);
                bikeAlreadyHits.Add(e);
            }
        }
        ModifyOverclockCharge(-10 * Time.deltaTime);
    }
    void StopBikeLogic()
    {
        isBikeRunning = false;
        player.es.SetEntityState(EntityStats.EntityStates.Neutral);
        player.es.gm.ActivePlayerWeapon.SendAnimatorBool("IsBike", false);
    }
}
