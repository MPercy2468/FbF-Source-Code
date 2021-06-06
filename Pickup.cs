using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    StatusEffectData[] StatusEffectData;
    [SerializeField]
    bool VaccumToHealth, VaccumToBallisticAmmo,VaccumToEnergyAmmo,VaccumToSpecialAmmo,AlwaysVaccum;
    float VaccumRange = 6;
    GameMaster gm;
    Rigidbody2D rb;
    float LerpSpeed = 0.3f, SlowdownLerpSpeed = 0.3f;
    float MoveSpeed = 0;
    bool WasPickedUp,CanMove;
    [SerializeField]
    string pickupSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (WasPickedUp)
        {
            return;
        }
        if (collision.transform.gameObject.GetComponent<EntityStats>()!= null)
        {
            WasPickedUp = true;
            EntityStats es = collision.transform.gameObject.GetComponent<EntityStats>();
            for (int i = 0; i < StatusEffectData.Length; i++)
            {
                StatusEffectData EffectData = StatusEffectData[i];
                es.AddStatusEffect(EffectData.Name, EffectData.Duration, EffectData.Magnitude, EffectData.EntityType, EffectData.AmmoType,EffectData.Direction);
                es.ac.PlaySound(pickupSound, AudioMaster.SoundTypes.SFX, true, false);
            }
        }
        DestroyPickup();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (WasPickedUp)
        {
            return;
        }
        if (collision.transform.gameObject.GetComponent<EntityStats>() != null)
        {
            WasPickedUp = true;
            EntityStats es = collision.transform.gameObject.GetComponent<EntityStats>();
            for (int i = 0; i < StatusEffectData.Length; i++)
            {
                StatusEffectData EffectData = StatusEffectData[i];
                es.AddStatusEffect(EffectData.Name, EffectData.Duration, EffectData.Magnitude, EffectData.EntityType, EffectData.AmmoType,EffectData.Direction);
                es.ac.PlaySound(pickupSound, AudioMaster.SoundTypes.SFX, true, false);
            }
        }
        DestroyPickup();
    }
    private void Awake()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Vector2.Distance(gm.Player.transform.position, transform.position) < VaccumRange)
        {
            if (CanMove)
            {
                MoveToPlayer();
            }
        }
        else
        {
            LerpSpeed = 0.3f;
            SlowToStop();
            CanMove = false;
        }
        if (VaccumToHealth)
        {
            if (gm.Player.gameObject.GetComponent<EntityStats>().EntityHealth < gm.Player.gameObject.GetComponent<EntityStats>().EntityHealthMax)
            {
                SlowdownLerpSpeed = 0.3f;
                CanMove = true;
            }
        }
        if (VaccumToBallisticAmmo)
        {
            if (gm.GetAmmo(GameMaster.AmmoTypes.Ballistic) < gm.BallistcAmmoMax)
            {
                SlowdownLerpSpeed = 0.3f;
                CanMove = true;
            }
        }
        if (VaccumToEnergyAmmo)
        {
            if (gm.GetAmmo(GameMaster.AmmoTypes.Energy) < gm.EnergyAmmoMax)
            {
                SlowdownLerpSpeed = 0.3f;
                CanMove = true;
            }
        }
        if (VaccumToSpecialAmmo)
        {
            if (gm.GetAmmo(GameMaster.AmmoTypes.Special) < gm.SpecialAmmoMax)
            {
                SlowdownLerpSpeed = 0.3f;
                CanMove = true;
            }
        }
        if (AlwaysVaccum)
        {
            SlowdownLerpSpeed = 0.3f;
            CanMove = true;
        }
    }
    void MoveToPlayer()
    {
            LerpSpeed = Mathf.Lerp(LerpSpeed, 3, 0.5F * Time.deltaTime);
            MoveSpeed = Mathf.Lerp(MoveSpeed, 40, LerpSpeed * Time.deltaTime);
            Vector2 PlayerDir = (gm.Player.transform.position - transform.position).normalized;
            rb.velocity = PlayerDir * MoveSpeed;
    }
    void SlowToStop()
    {
        SlowdownLerpSpeed = Mathf.Lerp(LerpSpeed, 3, 0.5F * Time.deltaTime);
        MoveSpeed = Mathf.Lerp(MoveSpeed,0, SlowdownLerpSpeed * Time.deltaTime);
        Vector2 PlayerDir = (gm.Player.transform.position - transform.position).normalized;
        rb.velocity = PlayerDir * MoveSpeed;
    }
    void DestroyPickup()
    {
        Destroy(this.gameObject);
    }

}
