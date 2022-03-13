using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenPoint : GrapplePoint
{
    override protected void Awake()
    {
        base.Awake();
        useRaycastPosition = true;
        type = GrappleType.Green;
    }
    override public void OnPointHit()
    {
        return;
    }

    override public void OnPointReleased()
    {
        return;
    }
}