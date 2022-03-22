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
        onButtonPress.Invoke();
    }

    public override void OnPointReleased()
    {
        onButtonRelease.Invoke();
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        Color c = Color.black;
        c.a = .25f;
        Gizmos.color = c;
        base.OnDrawGizmos();
    }
#endif
}
