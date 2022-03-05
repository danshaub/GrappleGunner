using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrapplePoint : MonoBehaviour
{
    //Defines which kind of grapple interaction the grapple point will have.
    public enum GrappleType
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Orange = 3,
        OrangeDisabled = 4,
        Button = 5,
        None = 6
    }

    protected virtual void Awake() {
        gameObject.tag = "Hookable";
    }

    public GrappleType type {get; protected set;}
    public bool useRaycastPosition = true;
    public Vector3 grapplePosition;
    public Vector3 grappleRotation;

    public Vector3 GetGrapplePosition()
    {
        return transform.position + grapplePosition;
    }
    public Quaternion GetGrappleRotation()
    {
        return Quaternion.Euler(grappleRotation);
    }

    public abstract void OnPointHit();
    public abstract void OnPointReleased();
}
