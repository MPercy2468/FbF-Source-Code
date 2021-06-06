using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    protected EntityStats es;
    protected bool isDestroyed;
    private void Awake()
    {
        es = gameObject.GetComponent<EntityStats>();
    }
    private void Update()
    {
        if (!isDestroyed)
        {
            if (es.IsDead)
            {
                isDestroyed = true;
                DestroyObject();
            }
        }
    }
    public virtual void DestroyObject()
    {
        es.DestroyEntityShadows();
        es.DestroyEntitySpriteRenderers();
        es.DestroyColliders();
        es.gs.ExplodeGibs(es.LastDamageTaken.Direction);
    }
}
