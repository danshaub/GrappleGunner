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
}