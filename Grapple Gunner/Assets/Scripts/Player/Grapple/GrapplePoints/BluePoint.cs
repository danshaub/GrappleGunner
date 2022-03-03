using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePoint : GrapplePoint
{
    override protected void Awake()
    {
        base.Awake();
        type = GrappleType.Blue;
    }
    override public void OnPointHit()
    {
        return;
    }
}