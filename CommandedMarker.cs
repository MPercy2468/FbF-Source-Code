using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandedMarker : Marker
{
    [HideInInspector]
    public VanguardAI v;
    [HideInInspector]
    public EnforcerAI e;
    public override void MarkerLogic()
    {
        transform.position = Target.transform.position;
        if (Target.IsDead)
        {
            Destroy(gameObject);
        }
        if (e != null)
        {
            if (!e.IsCommanded)
            {
                Destroy(gameObject);
            }
        }
        else if(v != null)
        {
            if (!v.IsCommanded)
            {
                Destroy(gameObject);
            }
        }
    }
}
