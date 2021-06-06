using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibGFX : MonoBehaviour
{
    public SpriteRenderer[] renderers;
    float despawnCounter = 0;
    Color color;
    Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Vector2 pushDir = transform.position - collision.transform.position;
            pushDir = pushDir.normalized;
            rb.AddForce(pushDir * collision.gameObject.GetComponent<Rigidbody2D>().velocity*10);
        }
    }
    private void Start()
    {
        if (renderers[0] != null)
        {
            color = renderers[0].color;
            rb = GetComponent<Rigidbody2D>();
        }
        else
        {
            Destroy(this);
        }
    }
    private void Update()
    {
        if(despawnCounter >= 60)
        {
            color.a -= 0.3f * Time.deltaTime;
            foreach(SpriteRenderer s in renderers)
            {
                if (s != null)
                {
                    s.color = color;
                }
            }
        }
        else
        {
            despawnCounter += Time.deltaTime;
        }
        if (color.a < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
