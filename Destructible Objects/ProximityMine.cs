using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityMine : ExplosiveBarrel
{
    float mineTimeCounter = 0;
    float mineBeepTimeCounter = 0;
    float mineBeepTime = 1f;
    [SerializeField]
    Animator mineAnimator;
    private void Update()
    {
        if (isDestroyed)
        {
            return;
        }
        if (!isDestroyed)
        {
            if (es.IsDead)
            {
                isDestroyed = true;
                DestroyObject();
                return;
            }
        }
        if (Vector2.Distance(es.gm.Player.transform.position, transform.position) <= 2)
        {
            if (!isDestroyed)
            {
                isDestroyed = true;
                DestroyObject();
            }
        }
        else
        {
            if (isDestroyed)
            {
                return;
            }
            if(mineTimeCounter >= 6)
            {
                isDestroyed = true;
                DestroyObject();
            }
            else
            {
                mineTimeCounter += Time.deltaTime;
            }
        }
        if(mineTimeCounter >= 4.75f)
        {
            mineBeepTime = 0.1f;
        }
        if(mineBeepTimeCounter >= mineBeepTime)
        {
            mineAnimator.SetTrigger("IsBeep");
            es.ac.PlaySound("MineBeep", AudioMaster.SoundTypes.SFX, false, false);
            mineBeepTimeCounter = 0;
        }
        else
        {
            mineBeepTimeCounter += Time.deltaTime;
        }
    }
    public override void DestroyObject()
    {
        mineAnimator.SetTrigger("IsExplode");
        Instantiate(ExplosivePFX, transform.position, Quaternion.identity).SetActive(true);
        es.DestroyEntitySpriteRenderers();
        es.DestroyEntityShadows();
        es.DestroyColliders();
        es.ac.PlaySound("VoidExplosion", AudioMaster.SoundTypes.SFX, true, false);
        Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, barrelExplodeRadius,
    LayerMask.GetMask("Player"));
        for (int x = 0; x < Hits.Length; x++)
        {
            EntityStats Hit = Hits[x].gameObject.GetComponentInParent<EntityStats>();
            if (Hit != null)
            {
                Vector2 dir = (Hit.transform.position - transform.position).normalized;
                barrelDamage.Direction = dir;
                Hit.TakeDamage(barrelDamage);
            }
        }
    }
}
