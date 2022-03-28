using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPoint : GrapplePoint
{
    override protected void Awake()
    {
        base.Awake();
        type = GrappleType.Red;
    }
    override public void OnPointHit()
    {
        return;
    }

    override public void OnPointReleased()
    {
        return;
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        Color c = Color.red;
        c.a = .25f;
        Gizmos.color = c;
        base.OnDrawGizmos();
    }
#endif
}