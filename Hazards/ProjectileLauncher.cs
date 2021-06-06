using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField]
    GameObject projectile;
    [SerializeField]
    Damage damage;
    [SerializeField]
    Transform firePoint;
    [SerializeField]
    AudioController ac;
    [SerializeField]
    protected Animator launcherAnimator;
    GameMaster gm;
    [SerializeField]
    string launchSound;
    private void Start()
    {
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
    }
    public virtual IEnumerator LaunchProjectile(ProjectileHazard ph, bool hasDuration)
    {
        ac.PlaySound(launchSound, AudioMaster.SoundTypes.SFX, true, false);
        GameObject m = Instantiate(projectile, firePoint.transform.position, transform.rotation);
        m.GetComponent<Projectile>().rb.velocity = firePoint.transform.up * m.GetComponent<Projectile>().ProjectileSpeed;
        m.GetComponent<Projectile>().IsExplosive = true;
        m.GetComponent<Projectile>().ProjectileExplosionRadius = 5;
        m.GetComponent<Projectile>().ProjectileDamage = damage;
        m.GetComponent<Projectile>().HitPlayer = true;
        RaycastHit2D Hit = Physics2D.Raycast(firePoint.transform.position, firePoint.transform.up, 400, LayerMask.GetMask("Walls"));
        m.GetComponent<Projectile>().WallPosition = Hit.point;
        m.GetComponent<Projectile>().ProjectileDamage.Direction = transform.up;
        if (!hasDuration)
        {
            ph.StopHazard();
        }
        yield return null;
    }
}
