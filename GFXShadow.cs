using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GFXShadow : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer SourceRenderer, ShadowRenderer;
    Vector2 MaintainPos;
    private void Awake()
    {
        MaintainPos = transform.localPosition;
        transform.parent = null;
    }
    private void Update()
    {
        Vector2 TargetPos = (Vector2)SourceRenderer.transform.position + MaintainPos;
        transform.localPosition = TargetPos;
        transform.rotation = SourceRenderer.transform.rotation;
        ShadowRenderer.sprite = SourceRenderer.sprite;
        ShadowRenderer.enabled = SourceRenderer.enabled;
    }
}
