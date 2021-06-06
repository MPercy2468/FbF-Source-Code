using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTile : MonoBehaviour
{
    [HideInInspector]
    public ElectricHazard eh;
    [SerializeField]
    Animator anim;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        eh.RecieveDamageSignal(collision.GetComponent<EntityStats>());
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        eh.RecieveDamageSignal(collision.GetComponent<EntityStats>());
    }
    public void TurnOn()
    {
        anim.SetTrigger("On");
    }
    public void TurnOff()
    {
        anim.SetTrigger("Off");
    }
}
