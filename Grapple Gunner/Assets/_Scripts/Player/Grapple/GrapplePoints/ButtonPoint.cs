using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class ButtonPoint : GrapplePoint
{
    public UnityEvent onButtonPress;
    public UnityEvent onButtonRelease;

    private bool released = false;
    override protected void Awake()
    { 
        base.Awake();
        type = GrappleType.Button;
        useRaycastPosition = true;
    }
    override public void OnPointHit()
    {
        GetComponent<AudioSource>().Play();
        onButtonPress.Invoke();
        released = false;
    }

    public override void OnPointReleased()
    {
        if(released == true) return;

        released = true;
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
