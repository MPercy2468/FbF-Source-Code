using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursor : MonoBehaviour
{
    SpriteRenderer Renderer;
    GameMaster gm;
    [SerializeField]
    Sprite primaryCursor;
    [SerializeField]
    Sprite secondaryCursor;
    private void Awake()
    {
        Renderer = gameObject.GetComponent<SpriteRenderer>();
        gm = GameObject.Find("_GameMaster").GetComponent<GameMaster>();
    }
    private void FixedUpdate()
    {
        if (InputHandler.CurrentDevice == InputHandler.Devices.KBM)
        {
            Renderer.enabled = true;
            Vector2 MousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            MousePos = Camera.main.ScreenToWorldPoint(MousePos);
            transform.position = MousePos;
        }
        else
        {
            Renderer.enabled = false;
        }
    }
    private void Update()
    {
        if (gm != null)
        {
            if (gm.ActivePlayerWeapon.AltFire)
            {
                Renderer.sprite = secondaryCursor;
            }
            else
            {
                Renderer.sprite = primaryCursor;
            }
            if (gm.um.UIState != UIMaster.UIStates.PlayerInfo ||gm.Player.es.IsDead)
            {
                Renderer.enabled = false;
            }
            else
            {
                switch (gm.ActivePlayerWeapon.AmmoType)
                {
                    case GameMaster.AmmoTypes.Ballistic:
                        Renderer.color = Color.yellow;
                            break;
                    case GameMaster.AmmoTypes.Energy:
                        Renderer.color = Color.cyan;
                        break;
                    case GameMaster.AmmoTypes.Special:
                        Renderer.color = Color.red;
                        break;
                }
            }
        }
    }
}
