using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenPoint : GrapplePoint
{
    public bool playerCollided { get; private set; }
    override protected void Awake()
    {
        base.Awake();
        useRaycastPosition = true;
        type = GrappleType.Green;
    }
    override public void OnPointHit()
    {
        return;
    }

    override public void OnPointReleased()
    {
        return;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCollided = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCollided = false;
        }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        Color c = Color.green;
        c.a = .25f;
        Gizmos.color = c;
        base.OnDrawGizmos();
    }
#endif
}