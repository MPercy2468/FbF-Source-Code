using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public enum MeleeType
    {
        Sabre,
        Spear,
        Axe,
        Sabre2
    }
    [SerializeField]
    GameObject swordLight,axeLight,spearLight;
    [SerializeField]
    GameObject swordSurgeProjectile, axeSurgeProjectile, spearSurgeProjectile;
    int meleeSwingValue = 2;
    [SerializeField]
    Transform meleeTransform;
    List<EntityStats> meleeAlreadyHits = new List<EntityStats>();
    private void Start()
    {
        swordLight.SetActive(false);
        spearLight.SetActive(false);
        axeLight.SetActive(false);
    }

    public IEnumerator StartMelee(Player player,MeleeType meleeType)
    {
        if(meleeType == MeleeType.Spear)
        {
            yield return StartCoroutine(SpearLogic(player));
        }
        else if(meleeType == MeleeType.Axe)
        {
            yield return StartCoroutine(AxeLogic(player));
        }
        else if(meleeType == MeleeType.Sabre2)
        {

        }
        else
        {
            yield return StartCoroutine(SwordLogic(player));
        }
    }

    public IEnumerator StartChargedMelee(Player player, MeleeType meleeType,float chargeTime)
    {
        if (meleeType == MeleeType.Spear)
        {
            yield return StartCoroutine(ChargedSpearLogic(player,chargeTime));
        }
        else if (meleeType == MeleeType.Axe)
        {
            yield return StartCoroutine(AxeLogic(player));
        }
        else if (meleeType == MeleeType.Sabre2)
        {

        }
        else
        {
            yield return StartCoroutine(SwordLogic(player));
        }
    }

    IEnumerator SwordLogic(Player player)
    {
        SpriteRenderer renderer = player.es.gm.ActivePlayerWeapon.WeaponRenderer;
        renderer.material = player.es.gm.swordMaterial;
        player.es.SetEntityState(EntityStats.EntityStates.Melee);
        player.es.gm.ActivePlayerWeapon.isTempLaserDisable = true;
        player.es.ac.PlaySound("Blade1", AudioMaster.SoundTypes.SFX, true, false);
        swordLight.SetActive(true);
        switch (meleeSwingValue)
        {
            case 1:
                meleeSwingValue = 2;
                break;
            case 2:
                meleeSwingValue = 1;
                break;
        }
        player.es.gm.ActivePlayerWeapon.SendAnimatorInt("IsBlade", meleeSwingValue);
        //Plasma Surge
        if (player.overclock.isPlasmaSurge)
        {
            player.overclock.ModifyOverclockCharge(-28);
            GameObject surge = Instantiate(swordSurgeProjectile, meleeTransform.position, transform.rotation);
            Projectile p = surge.GetComponent<Projectile>();
            p.rb.velocity = meleeTransform.up * p.ProjectileSpeed;
            p.ProjectileDamage = player.overclock.plasmaSurgeDamage;
            RaycastHit2D Hit = Physics2D.Raycast(meleeTransform.position, meleeTransform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            p.ProjectileDamage.Direction = meleeTransform.up;
        }
        //Melee Slice
        for (float i = 0; i <= 0.150f;)
        {
            Vector2 HitboxSize = new Vector2(2.4f, 1.5f);
            Collider2D[] Hits = Physics2D.OverlapBoxAll(meleeTransform.position,
                HitboxSize, transform.rotation.eulerAngles.z, LayerMask.GetMask("Enemies") + LayerMask.GetMask("Destructables"));
            for (int x = 0; x < Hits.Length; x++)
            {
                EntityStats Hit = Hits[x].gameObject.GetComponentInParent<EntityStats>();
                if (Hit != null)
                {
                    if (!meleeAlreadyHits.Contains(Hit))
                    {
                        meleeAlreadyHits.Add(Hit);
                        Damage d = null;
                        d = new Damage(20, true, true, false, false, Damage.damageTypeTagsEnum.Melee, Damage.damageSourceTagsEnum.PlasmaSabre);
                        d.Direction = transform.up;
                        Hit.TakeDamage(d);
                    }
                }
            }
            i += 1 * Time.deltaTime;
            yield return null;
        }
        meleeAlreadyHits.Clear();
        swordLight.SetActive(false);
        yield return new WaitForSeconds(0.095f);
        renderer.material = player.es.gm.defaultWeaponMaterial;
        player.es.gm.ActivePlayerWeapon.SendAnimatorInt("IsBlade", 0);
        player.es.gm.ActivePlayerWeapon.isTempLaserDisable = false;
        player.es.SetEntityState(EntityStats.EntityStates.Neutral);
    }
    IEnumerator ChargedSwordLogic(Player player)
    {
        player.es.ac.PlaySound("Blade1", AudioMaster.SoundTypes.SFX, true, false);
        swordLight.SetActive(true);
        switch (meleeSwingValue)
        {
            case 1:
                meleeSwingValue = 2;
                break;
            case 2:
                meleeSwingValue = 1;
                break;
        }
        player.es.gm.ActivePlayerWeapon.SendAnimatorInt("IsBlade", meleeSwingValue);
        //Plasma Surge
        if (player.overclock.isPlasmaSurge)
        {
            player.overclock.ModifyOverclockCharge(-28);
            GameObject surge = Instantiate(swordSurgeProjectile, meleeTransform.position, transform.rotation);
            Projectile p = surge.GetComponent<Projectile>();
            p.rb.velocity = meleeTransform.up * p.ProjectileSpeed;
            p.ProjectileDamage = player.overclock.plasmaSurgeDamage;
            RaycastHit2D Hit = Physics2D.Raycast(meleeTransform.position, meleeTransform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            p.ProjectileDamage.Direction = meleeTransform.up;
        }
        //Melee Slice
        for (float i = 0; i <= 0.150f;)
        {
            Vector2 HitboxSize = new Vector2(2.4f, 1.5f);
            Collider2D[] Hits = Physics2D.OverlapBoxAll(meleeTransform.position,
                HitboxSize, transform.rotation.eulerAngles.z, LayerMask.GetMask("Enemies") + LayerMask.GetMask("Destructables"));
            for (int x = 0; x < Hits.Length; x++)
            {
                EntityStats Hit = Hits[x].gameObject.GetComponentInParent<EntityStats>();
                if (Hit != null)
                {
                    if (!meleeAlreadyHits.Contains(Hit))
                    {
                        meleeAlreadyHits.Add(Hit);
                        Damage d = null;
                        d = new Damage(5, true, true, false, false, Damage.damageTypeTagsEnum.Melee, Damage.damageSourceTagsEnum.PlasmaSabre);
                        d.Direction = transform.up;
                        Hit.TakeDamage(d);
                    }
                }
            }
            i += 1 * Time.deltaTime;
            yield return null;
        }
        meleeAlreadyHits.Clear();
        swordLight.SetActive(false);
        yield return new WaitForSeconds(0.095f);
        player.es.gm.ActivePlayerWeapon.SendAnimatorInt("IsBlade", 0);
    }

    IEnumerator SpearLogic(Player player)
    {
        SpriteRenderer renderer = player.es.gm.ActivePlayerWeapon.WeaponRenderer;
        renderer.material = player.es.gm.spearMaterial;
        player.es.SetEntityState(EntityStats.EntityStates.Melee);
        player.es.gm.ActivePlayerWeapon.isTempLaserDisable = true;
        player.es.ac.PlaySound("Blade1", AudioMaster.SoundTypes.SFX, true, false);
        spearLight.SetActive(true);
        player.es.gm.ActivePlayerWeapon.SendAnimatorInt("IsSpear", 1);
        /*
        //Plasma Surge
        if (player.overclock.isPlasmaSurge)
        {
            player.overclock.ModifyOverclockCharge(-28);
            GameObject surge = Instantiate(swordSurgeProjectile, meleeTransform.position, transform.rotation);
            Projectile p = surge.GetComponent<Projectile>();
            p.rb.velocity = meleeTransform.up * p.ProjectileSpeed;
            p.ProjectileDamage = player.overclock.plasmaSurgeDamage;
            RaycastHit2D Hit = Physics2D.Raycast(meleeTransform.position, meleeTransform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            p.ProjectileDamage.Direction = meleeTransform.up;
        }
        */
        //Spear Stab
        for (float i = 0; i <= 0.100f;)
        {
            Vector2 HitboxSize = new Vector2(0.8f, 3.5f);
            Collider2D[] Hits = Physics2D.OverlapBoxAll(meleeTransform.position+transform.up*1.4f,
                HitboxSize, transform.rotation.eulerAngles.z, LayerMask.GetMask("Enemies") + LayerMask.GetMask("Destructables"));
            for (int x = 0; x < Hits.Length; x++)
            {
                EntityStats Hit = Hits[x].gameObject.GetComponentInParent<EntityStats>();
                if (Hit != null)
                {
                    if (!meleeAlreadyHits.Contains(Hit))
                    {
                        meleeAlreadyHits.Add(Hit);
                        Damage d = null;
                        d = new Damage(20, true, true, false, false, Damage.damageTypeTagsEnum.Melee, Damage.damageSourceTagsEnum.PlasmaSpear);
                        d.Direction = transform.up;
                        Hit.TakeDamage(d);
                    }
                }
            }
            i += 1 * Time.deltaTime;
            yield return null;
        }
        meleeAlreadyHits.Clear();
        spearLight.SetActive(false);
        yield return new WaitForSeconds(0.145f);
        renderer.material = player.es.gm.defaultWeaponMaterial;
        player.es.gm.ActivePlayerWeapon.SendAnimatorInt("IsSpear", 0);
        player.es.SetEntityState(EntityStats.EntityStates.Neutral);
        player.es.gm.ActivePlayerWeapon.isTempLaserDisable = false;
    }
    IEnumerator ChargedSpearLogic(Player player,float chargeTime)
    {
        Debug.Log("LANCE TIME");
        //Player charges in direction she is facing
        player.canMove = false;
        player.es.rb.velocity = player.transform.up * 30;
        //Deals damage in front of her as her spear is out
        spearLight.SetActive(true);
        player.es.SetEntityState(EntityStats.EntityStates.Melee);
        for (float i = 0; i <= 1.5f; i+=Time.deltaTime)
        {
            Vector2 HitboxSize = new Vector2(0.8f, 3.5f);
            Collider2D[] Hits = Physics2D.OverlapBoxAll(meleeTransform.position + transform.up * 1.4f,
                HitboxSize, transform.rotation.eulerAngles.z, LayerMask.GetMask("Enemies") + LayerMask.GetMask("Destructables"));
            for (int x = 0; x < Hits.Length; x++)
            {
                EntityStats Hit = Hits[x].gameObject.GetComponentInParent<EntityStats>();
                if (Hit != null)
                {
                    if (!meleeAlreadyHits.Contains(Hit))
                    {
                        meleeAlreadyHits.Add(Hit);
                        Damage d = null;
                        d = new Damage(20, true, true, false, false, Damage.damageTypeTagsEnum.Melee, Damage.damageSourceTagsEnum.PlasmaSpear);
                        d.Direction = transform.up;
                        Hit.TakeDamage(d);
                    }
                }
            }
        }
        player.canMove = true;
        spearLight.SetActive(false);
        yield return null;
    }

    IEnumerator AxeLogic(Player player)
    {
        SpriteRenderer renderer = player.es.gm.ActivePlayerWeapon.WeaponRenderer;
        renderer.material = player.es.gm.axeMaterial;
        player.es.SetEntityState(EntityStats.EntityStates.Melee);
        player.es.gm.ActivePlayerWeapon.isTempLaserDisable = true;
        player.es.ac.PlaySound("Blade1", AudioMaster.SoundTypes.SFX, true, false);
        axeLight.SetActive(true);
        player.es.gm.ActivePlayerWeapon.SendAnimatorInt("IsAxe", 1);
        /*
        //Plasma Surge
        if (player.overclock.isPlasmaSurge)
        {
            player.overclock.ModifyOverclockCharge(-28);
            GameObject surge = Instantiate(swordSurgeProjectile, meleeTransform.position, transform.rotation);
            Projectile p = surge.GetComponent<Projectile>();
            p.rb.velocity = meleeTransform.up * p.ProjectileSpeed;
            p.ProjectileDamage = player.overclock.plasmaSurgeDamage;
            RaycastHit2D Hit = Physics2D.Raycast(meleeTransform.position, meleeTransform.up, 400f, LayerMask.GetMask("Walls"));
            p.WallPosition = Hit.point;
            p.ProjectileDamage.Direction = meleeTransform.up;
        }
        */
        //Axe swing hitbox
        for (float i = 0; i <= 0.150f;)
        {
            Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, 3, LayerMask.GetMask("Enemies") + LayerMask.GetMask("Destructables"));
            for (int x = 0; x < Hits.Length; x++)
            {
                EntityStats Hit = Hits[x].gameObject.GetComponentInParent<EntityStats>();
                if (Hit != null)
                {
                    if (!meleeAlreadyHits.Contains(Hit))
                    {
                        meleeAlreadyHits.Add(Hit);
                        Damage d = null;
                        d = new Damage(20, true, true, false, false, Damage.damageTypeTagsEnum.Melee, Damage.damageSourceTagsEnum.PlasmaAxe);
                        d.Direction = transform.up;
                        Hit.TakeDamage(d);
                    }
                }
            }
            i += 1 * Time.deltaTime;
            yield return null;
        }
        meleeAlreadyHits.Clear();
        axeLight.SetActive(false);
        yield return new WaitForSeconds(0.145f);
        renderer.material = player.es.gm.defaultWeaponMaterial;
        player.es.gm.ActivePlayerWeapon.SendAnimatorInt("IsAxe", 0);
        player.es.SetEntityState(EntityStats.EntityStates.Neutral);
        player.es.gm.ActivePlayerWeapon.isTempLaserDisable = false;
    }
    IEnumerator ChargedAxeLogic(Player player)
    {
        yield return null;
    }
}
