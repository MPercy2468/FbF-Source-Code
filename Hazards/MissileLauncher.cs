using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : ProjectileLauncher
{
    public override IEnumerator LaunchProjectile(ProjectileHazard ph, bool hasDuration)
    {
        launcherAnimator.SetTrigger("isOpen");
        yield return new WaitForSeconds(0.6f);
        yield return base.LaunchProjectile(ph, hasDuration);
    }
}
