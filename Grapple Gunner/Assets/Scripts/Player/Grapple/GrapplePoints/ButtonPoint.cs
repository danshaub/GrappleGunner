using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonPoint : GrapplePoint
{
    override protected void Start()
    { 
        base.Start();
        type = GrappleType.Button;
    }
    override public void OnPointHit()
    {
        return;
    }
}