using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHazard : Hazard
{
    LineRenderer laserLineRenderer;
    [SerializeField]
    Transform laserStartPoint, laserEndPoint;
    [SerializeField]
    BoxCollider2D laserCollider;
    float laserThickness = 1;
    float laserLength;
    bool isEnd;
    private void Awake()
    {
        laserLineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    public override void StartHazard()
    {
        //set line renderer values
        laserLineRenderer.widthMultiplier = 0.1f;
        laserLineRenderer.enabled = true;
        DrawLaser();
        //set collider values
        laserCollider.size = new Vector2(laserLength, laserLineRenderer.widthMultiplier);
        laserCollider.gameObject.transform.position = laserStartPoint.transform.position + laserStartPoint.transform.up * (laserLength / 2);
        laserCollider.enabled = true;
        laserStartPoint.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        laserEndPoint.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        hazardState = hazardStatesEnum.on;
        ac.PlaySound("HazardLaser", AudioMaster.SoundTypes.SFX, false, true);
    }
    public override void StopHazard()
    {
        laserCollider.enabled = false;
        hazardState = hazardStatesEnum.off;
        ac.StopSound("HazardLaser");
    }
    public override void EndHazard()
    {
        Debug.Log("Called?");
        isEnd = true;
    }
    void DrawLaser()
    {
        laserLength = Vector2.Distance(laserStartPoint.position, laserEndPoint.position);
        laserLineRenderer.SetPosition(0, laserStartPoint.position);
        laserLineRenderer.SetPosition(1, laserStartPoint.position + laserStartPoint.up * (laserLength / 2));
        laserLineRenderer.SetPosition(2, laserEndPoint.position);
    }
    public override void HazardUpdateLogic()
    {
        if (hazardState == hazardStatesEnum.on)
        {
            DrawLaser();
            laserLineRenderer.widthMultiplier = Mathf.Lerp(laserLineRenderer.widthMultiplier, laserThickness, 5 * Time.deltaTime);
            laserCollider.size = new Vector2(laserLength, laserLineRenderer.widthMultiplier);
        }
        else if(hazardState == hazardStatesEnum.off)
        {
            laserLineRenderer.widthMultiplier = Mathf.Lerp(laserLineRenderer.widthMultiplier, 0, 5 * Time.deltaTime);
            if(laserLineRenderer.widthMultiplier <= 0.1f)
            {
                laserLineRenderer.enabled = false;
                laserStartPoint.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                laserEndPoint.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                hazardState = hazardStatesEnum.neutral;
                if (isEnd)
                {
                    return;
                }
                if (canRepeat)
                {
                    Debug.Log("Invoke in: " + timeToStart);
                    Invoke("StartHazard", timeToStart);
                }
            }
        }
    }
}
