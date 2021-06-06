using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGrenade : Grenade
{
    public override IEnumerator Explode()
    {
        Debug.Log("Start explosion");
        rb.velocity = Vector2.zero;
        WasExploded = true;
        Collider2D[] Hits = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius,
        LayerMask.GetMask("Enemies", "Player"));
        List<EntityStats> AlreadyHits = new List<EntityStats>();
        GrenadeAnimator.SetTrigger("Explode");
        GrenadePFX.SetActive(true);
        GrenadePFX.GetComponentInChildren<AudioController>().PlaySound("HEExplosion", AudioMaster.SoundTypes.SFX, true,false);
        for (int x = 0; x < Hits.Length; x++)
        {
            EntityStats Hit = Hits[x].gameObject.GetComponentInParent<EntityStats>();
            if (Hit != null)
            {
                if (!AlreadyHits.Contains(Hit))
                {
                    AlreadyHits.Add(Hit);
                    GrenadeDamage.UpdateDirection((Hit.transform.position - transform.position).normalized);
                    if (Hit.EntityHealth - GrenadeDamage.DamageAmount <= 0)
                    {
                        Hit.FinisherKill(GrenadeDamage.CanHealthFinish, GrenadeDamage.CanAmmoFinish);
                    }
                    Hit.TakeDamage(GrenadeDamage);
                }
            }
        }
        yield return new WaitForSeconds(0.55f);
        GrenadeRenderer.sortingLayerName = "GroundEffects";
    }
}
