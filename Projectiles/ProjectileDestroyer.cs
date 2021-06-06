using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDestroyer : MonoBehaviour
{
    public void DestroyProjectile()
    {
        Destroy(transform.parent.gameObject);
    }
}
