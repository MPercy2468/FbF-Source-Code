using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [HideInInspector]
    public EntityStats Target;
    private void Update()
    {
        MarkerLogic();
    }
    public virtual void MarkerLogic()
    {
        transform.position = Target.transform.position;
        if (Target.IsDead)
        {
            Destroy(gameObject);
        }
    }
}
