using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibSystem : MonoBehaviour
{
    [SerializeField]
    Sprite[] gibs;
    EntityStats es;
    private void Awake()
    {
        es = gameObject.GetComponent<EntityStats>();
    }
    public void ExplodeGibs(Vector2 dir)
    {
        int gibCount = Random.Range(1, gibs.Length+1);
        Vector2 gibDir = Vector2.zero;
        SplatterBlood(10);
        for(int i = 0; i < gibCount; i++)
        {
            float rot = Random.Range(0, 360);
            Quaternion gibRot = Quaternion.Euler(new Vector3(0, 0, rot));
            GameObject gib = Instantiate(es.gm.gibTemplatePrefab, transform.position,gibRot);
            gib.GetComponent<SpriteRenderer>().sprite = gibs[i];
            if(dir != Vector2.zero)
            {
                float RadShotSpread = 60 * 0.0174533f;
                float SpreadX = Random.Range(dir.x - RadShotSpread, dir.x + RadShotSpread);
                float SpreadY = Random.Range(dir.y - RadShotSpread, dir.y + RadShotSpread);
                gibDir = new Vector2(SpreadX, SpreadY).normalized;
            }
            else
            {
                gibDir = Random.insideUnitCircle.normalized;
            }
            gib.GetComponent<Rigidbody2D>().AddForce(gibDir * 650);
        }
    }
    public void SplatterBlood(int splatterCount)
    {
        float rot = Random.Range(0, 360);
        Quaternion bloodRot = Quaternion.Euler(new Vector3(0, 0, rot));
        Vector2 bloodPos;
        if (es.EntityType == EntityStats.EntityTypes.Organic)
        {
            for (int i = 0; i < splatterCount; i++)
            {
                bloodPos = (Vector2)transform.position + Random.insideUnitCircle * 2;
                int r = Random.Range(0, es.gm.OrganicGore.Length);
                Instantiate(es.gm.OrganicGore[r], bloodPos, bloodRot);
            }
        }
        else if (es.EntityType == EntityStats.EntityTypes.Cybernetic)
        {
            for (int i = 0; i < splatterCount; i++)
            {
                bloodPos = (Vector2)transform.position + Random.insideUnitCircle * 2;
                int r = Random.Range(0, es.gm.CyberneticGore.Length);
                Instantiate(es.gm.CyberneticGore[r], bloodPos, bloodRot);
            }
        }
    }
}
