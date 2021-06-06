using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    EntityStats es;

    private void Awake()
    {
        es = gameObject.GetComponent<EntityStats>();
    }

}
