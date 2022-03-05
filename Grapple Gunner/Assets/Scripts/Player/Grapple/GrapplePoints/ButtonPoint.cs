using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ButtonPoint : GrapplePoint
{
    public UnityEvent onButtonPress;
    public UnityEvent onButtonRelease;
    override protected void Awake()
    { 
        base.Awake();
        type = GrappleType.Button;
        useRaycastPosition = true;
    }
    override public void OnPointHit()
    {
        Debug.Log("Button Hit");
        onButtonPress.Invoke();
    }

    public override void OnPointReleased()
    {
        Debug.Log("Button Released");
        onButtonRelease.Invoke();
    }
}
