using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangePoint : GrapplePoint
{
    override protected void Start()
    {
        base.Start();
        type = GrappleType.Orange;
    }
    override public void OnPointHit()
    {
        return;
    }
}