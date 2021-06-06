using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : DestructableObject
{
    [SerializeField]
    protected GameObject ExplosivePFX;
    [SerializeField]
    protected Damage barrelDamage;
    [SerializeField]
    protected float barrelExplodeRadius;

    public override void DestroyObject()
    {
        Instantiate(ExplosivePFX, transform.position, Quaternion.identity).SetActive(true);
        es.DestroyEntitySpriteRenderers();
        es.DestroyEntityShadows();
        es.DestroyColliders();
        es.gs.ExplodeGibs(es.LastDamageTaken.Direction);
        Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, barrelExplodeRadius,
    LayerMask.GetMask("Enemies", "Player","Destructables"));
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
