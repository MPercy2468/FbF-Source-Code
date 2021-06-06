using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHazard : Hazard
{
    [SerializeField]
    ProjectileLauncher[] projectileLaunchers;
    [SerializeField]
    bool isRandomLauncher;
    int randomLaunchCount;
    int launcherIndex;
    [HideInInspector]
    public bool canLaunch;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
    private void Start()
    {
        hazardState = hazardStatesEnum.off;
        if (isSelfStart)
        {
            StartHazard();
        }
    }
    public override void EndHazard()
    {
        Debug.Log("Stop Missile");
        hazardState = hazardStatesEnum.off;
        if (canRepeat)
        {
            Invoke("StartHazard", timeToStart);
        }
    }

    public override void HazardUpdateLogic()
    {
        if (hazardState == hazardStatesEnum.on)
        {
            hazardState = hazardStatesEnum.neutral;
            if(launcherIndex >= projectileLaunchers.Length)
            {
                EndHazard();
                return;
            }
            else if (isRandomLauncher)
            {
                if(randomLaunchCount >= projectileLaunchers.Length)
                {
                    EndHazard();
                    return;
                }
            }
            StartCoroutine(projectileLaunchers[launcherIndex].LaunchProjectile(this,hasDuration));
            if (isRandomLauncher)
            {
                launcherIndex = Random.Range(0, projectileLaunchers.Length);
                randomLaunchCount++;
            }
            else
            {
                launcherIndex++;
            }

        }
    }

    public override void StartHazard()
    {
        if (hazardState == hazardStatesEnum.off)
        {
            Debug.Log("Start missile");
            hazardState = hazardStatesEnum.on;
            launcherIndex = 0;
            randomLaunchCount = 0;
        }
    }

    public override void StopHazard()
    {
        Debug.Log("Next Missile");
        hazardState = hazardStatesEnum.on;
    }
    public override void DurationTick()
    {
        if (!hasDuration)
        {
            return;
        }
        if (hazardState == hazardStatesEnum.neutral)
        {
            if (hazardDurationCounter >= hazardDurationTime)
            {
                hazardDurationCounter = 0;
                StopHazard();
            }
            hazardDurationCounter += Time.deltaTime;
        }
    }
}
