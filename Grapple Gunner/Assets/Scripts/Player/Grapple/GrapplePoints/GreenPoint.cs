using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenPoint : GrapplePoint
{
    override protected void Start()
    {
        base.Start();
        type = GrappleType.Green;
    }
    override public void OnPointHit()
    {
        return;
    }
}