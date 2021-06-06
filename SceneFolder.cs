using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFolder : MonoBehaviour
{
    private void Awake()
    {
        gameObject.transform.DetachChildren();
        Destroy(gameObject);
    }
}
