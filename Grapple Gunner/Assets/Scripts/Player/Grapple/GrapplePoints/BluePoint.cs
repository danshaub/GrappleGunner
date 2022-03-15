using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BluePoint : GrapplePoint
{
    override protected void Awake()
    {
        base.Awake();
        type = GrappleType.Blue;
        useRaycastPosition = true;
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