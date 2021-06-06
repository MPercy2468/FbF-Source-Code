using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBehavior : MonoBehaviour
{
    protected WeaponStats ws;
    protected bool PrimaryReleased, AltReleased, IsFirePointInWall;
    private void Awake()
    {
        ws = gameObject.GetComponent<WeaponStats>();
    }
    private void Update()
    {
        /*
        DashEffectUpdate();
        GetWeaponInputs();
        ShowWeaponLaser();
        if (ws.TimeToFire < ws.FireRate)
        {
            ws.TimeToFire += 1 * Time.deltaTime;
        }
        if (ws.AltTimeToFire < ws.AltFireRate)
        {
            ws.AltTimeToFire += 1 * Time.deltaTime;
        }
        */
    }
    protected void DashEffectUpdate()
    {
        if (!ws.IsActive)
        {
            return;
        }
        ws.DashEffect.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, ws.WeaponRenderer.sprite);
    }
    public virtual void GetWeaponInputs()
    {
        /*
        if (ws.gm.Player.gameObject.GetComponent<EntityStats>().IsDead)
        {
            return;
        }
        if (ws.gm.um.UIState != UIMaster.UIStates.PlayerInfo)
        {
            return;
        }
        if(ws.gm.Player.CurrentOverclockState == Player.OverclockStates.On&&ws.gm.Player.SelectedOverclockState == Player.OverclockStates.Blue
            &&ws.gm.Player.es.EntityState == EntityStats.EntityStates.Dashing)
        {
            return;
        }
        if (!ws.IsActive)
        {
            return;
        }
        if (InputHandler.GetAction(ws.gm.gc.Player.Shoot))
        {
            if (ws.AltFire)
            {
                if (ws.AltPressInput)
                {
                    if (AltReleased)
                    {
                        AltReleased = false;
                        if (ws.AltTimeToFire >= ws.AltFireRate && ws.IsActive)
                        {
                            ws.AltTimeToFire = 0;
                            if (ws.gm.CheckAmmo(ws.AmmoCost, ws.AmmoType) >= 0)
                            {
                                StartCoroutine(AltShoot());
                            }
                        }
                    }
                }
                else
                {
                    if (ws.AltTimeToFire >= ws.AltFireRate && ws.IsActive)
                    {
                        ws.AltTimeToFire = 0;
                        if (ws.gm.CheckAmmo(ws.AmmoCost, ws.AmmoType) >= 0)
                        {
                            StartCoroutine(AltShoot());
                        }
                    }
                }
            }
            else
            {
                if (ws.PrimaryPressInput)
                {
                    if (PrimaryReleased)
                    {
                        PrimaryReleased = false;
                        if (ws.TimeToFire >= ws.FireRate && ws.IsActive)
                        {
                            ws.TimeToFire = 0;
                            if (ws.gm.CheckAmmo(ws.AmmoCost, ws.AmmoType) >= 0)
                            {
                                StartCoroutine(Shoot());
                            }
                        }
                    }
                }
                else
                {
                    if (ws.TimeToFire >= ws.FireRate && ws.IsActive)
                    {
                        ws.TimeToFire = 0;
                        if(ws.gm.CheckAmmo(ws.AmmoCost, ws.AmmoType) >= 0)
                        {
                            StartCoroutine(Shoot());
                        }
                    }
                }

            }
        }
        else
        {
            if (!PrimaryReleased)
            {
                PrimaryReleased = true;
            }
            if (!AltReleased)
            {
                AltReleased = true;
            }
        }
        */
    }
    protected void ToggleAltFire()
    {
        if (ws.IsActive)
        {
            ws.AltFire = !ws.AltFire;
        }
    }
    public virtual void ShowWeaponLaser()
    {
        /*
        if (!ws.IsActive||ws.gm.Player.GetComponent<EntityStats>().IsDead)
        {
            ws.PrimaryLaser.Laser.enabled = false;
            ws.AltLaser.Laser.enabled = false;
            return;
        }
        if (ws.AltFire)
        {
            ws.PrimaryLaser.Laser.enabled = false;
            ws.AltLaser.Laser.SetPosition(0, ws.FirePoint.transform.position + ws.FirePoint.transform.up * ws.AltLaser.StartDistance);
            ws.AltLaser.Laser.SetPosition(1, ws.FirePoint.transform.position + transform.up * (ws.AltLaser.LaserLength/2));
            ws.AltLaser.Laser.SetPosition(2, ws.FirePoint.transform.position + transform.up * ws.AltLaser.LaserLength);
            ws.AltLaser.Laser.enabled = true;
        }
        else
        {
            ws.AltLaser.Laser.enabled = false;
            ws.PrimaryLaser.Laser.SetPosition(0, ws.FirePoint.transform.position + ws.FirePoint.transform.up * ws.PrimaryLaser.StartDistance);
            ws.PrimaryLaser.Laser.SetPosition(1, ws.FirePoint.transform.position + transform.up * (ws.PrimaryLaser.LaserLength/2));
            ws.PrimaryLaser.Laser.SetPosition(2, ws.FirePoint.transform.position + transform.up * ws.PrimaryLaser.LaserLength);
            ws.PrimaryLaser.Laser.enabled = true;
        }
        */
    }
    public abstract IEnumerator Shoot();
    public abstract IEnumerator AltShoot();
}
