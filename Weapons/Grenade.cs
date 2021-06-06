using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grenade : MonoBehaviour
{
    [SerializeField]
    protected GameObject GrenadePFX;
    [SerializeField]
    protected AudioController ac;
    [SerializeField]
    protected Animator GrenadeAnimator;
    [SerializeField]
    protected SpriteRenderer GrenadeRenderer;
    [SerializeField]
    protected Damage GrenadeDamage;
    [SerializeField]
    protected float ExplosionRadius;
    public float ExplodeDistance;
    [SerializeField]
    protected float ThrowSpeed;
    protected Rigidbody2D rb;
    protected Vector2 ExplodePosition, ReflectionNormal;
    protected int Ricochets;
    protected bool WasExploded,WasThrown,CanExplode;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.gameObject.GetComponentInParent<EntityStats>() != null)
        {
            if (collision.transform.gameObject.GetComponentInParent<Player>() != null)
            {
                return;
            }
            if (CanExplode)
            {
                CanExplode = false;
                StartCoroutine(Explode());
            }
        }
    }
    private void Update()
    {
        if (WasExploded&&WasThrown)
        {
            return;
        }
        if(ExplodeDistance > 0)
        {
            ExplodeDistance -= 10 * Time.deltaTime;
        }
        if (Vector2.Distance(transform.position, ExplodePosition) < 0.5f)
        {
            Ricochet();
            if (CanExplode)
            {
                CanExplode = false;
                StartCoroutine(Explode());
            }
        }
        else
        {
            CanExplode = true;
        }
    }
    public void Throw(Vector2 ThrowDirection)
    {
        GrenadeRenderer.enabled = true;
        RaycastHit2D Hit = Physics2D.Raycast((Vector2)transform.position + ThrowDirection * 1, ThrowDirection, 
            ExplodeDistance, LayerMask.GetMask("Walls"));
        if(!Hit)
        {
            ExplodePosition = (Vector2)transform.position + ThrowDirection * ExplodeDistance;
            Ricochets = 0;
        }
        else
        {
            ExplodePosition = Hit.point;
            ReflectionNormal = Hit.normal;
            Ricochets = 1;
        }
        WasThrown = true;
        rb.velocity = ThrowDirection * ThrowSpeed;
    }
    public abstract IEnumerator Explode();
    protected void Ricochet()
    {
        if (Ricochets > 0)
        {
            Ricochets--;
            Vector2 RicochetDir = Vector2.Reflect(rb.velocity.normalized, ReflectionNormal);
            float Rotate = Mathf.Atan2(RicochetDir.y, RicochetDir.x);
            Rotate = Mathf.Rad2Deg * Rotate;
            Rotate -= 90;
            transform.eulerAngles = new Vector3(0, 0, Rotate);
            RaycastHit2D Hit = Physics2D.Raycast((Vector2)transform.position + RicochetDir * 1, RicochetDir,
                ExplodeDistance, LayerMask.GetMask("Walls"));
            if (!Hit)
            {
                ExplodePosition = (Vector2)transform.position + RicochetDir * ExplodeDistance;
                rb.velocity = RicochetDir * ThrowSpeed;
                Ricochets = 0;
            }
            else
            {
                ExplodePosition = Hit.point;
                ReflectionNormal = Hit.normal;
                rb.velocity = RicochetDir * ThrowSpeed;
                Ricochets = 1;
            }
            CanExplode = false;
        }
    }
}
