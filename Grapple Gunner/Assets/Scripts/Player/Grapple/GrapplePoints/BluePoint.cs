using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePoint : GrapplePoint
{
    override protected void Start()
    {
        base.Start();
        type = GrappleType.Blue;
    }
    override public void OnPointHit()
    {
        return;
    }
}