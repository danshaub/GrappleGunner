using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangePoint : GrapplePoint
{
    override protected void Awake()
    {
        base.Awake();
        type = GrappleType.Orange;
    }
    override public void OnPointHit()
    {
        return;
    }
}