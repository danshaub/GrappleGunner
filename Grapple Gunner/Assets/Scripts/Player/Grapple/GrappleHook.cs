using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GrappleHook : MonoBehaviour
{
    public enum GrappleState
    {
        None,
        Red,
        Green,
        Blue,
        Orange
    }

    public GrappleState state = GrappleState.None;
    public bool fired = false;
    public bool retracting = false;

    private float travelSpeed;
    private GrappleGun grappleGun;

    private Collider cd;
    private Rigidbody rb;
    private Transform returnTransform;
    private float retractInterpolateValue;
    private float snapReturnDistance = 0.5f;

    void Awake()
    {
        gameObject.tag = "Hook";
        cd = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (retracting)
        {
            transform.position = Vector3.Lerp(transform.position, returnTransform.position, retractInterpolateValue);
            transform.rotation = Quaternion.Slerp(transform.rotation, returnTransform.rotation, retractInterpolateValue);

            if (Vector3.Distance(transform.position, returnTransform.position) <= snapReturnDistance)
            {
                FinishRetract();
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        cd.enabled = false;
        if (other.gameObject.tag == "Hookable")
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.velocity = Vector3.zero;

            GrapplePoint gp = other.gameObject.GetComponent<GrapplePoint>();

            if (gp.useRaycastPosition)
            {
                transform.position = other.contacts[0].point;
                transform.rotation = Quaternion.LookRotation(-other.contacts[0].normal, Vector3.up);
            }
            else
            {
                transform.position = gp.getGrapplePosition();
                transform.rotation = gp.getGrappleRotation();
            }

            switch (gp.type)
            {
                case GrapplePoint.GrappleType.Red:
                    state = GrappleState.Red;
                    grappleGun.StartGrappleRed();
                    break;
                case GrapplePoint.GrappleType.Green:
                    state = GrappleState.Green;
                    grappleGun.StartGrappleGreen();
                    break;
                case GrapplePoint.GrappleType.Blue:
                    state = GrappleState.Blue;
                    grappleGun.StartGrappleBlue();
                    break;
                case GrapplePoint.GrappleType.Orange:
                    state = GrappleState.Orange;
                    grappleGun.StartGrappleOrange(gp.teleportParent, gp.teleportOffset, gp);
                    break;
                case GrapplePoint.GrappleType.Button:
                    gp.InvokeButtonEvent();
                    ReturnHook();
                    break;
                default:
                    break;
            }
        }
        else
        {
            ReturnHook();
        }
    }

    public void SetProperties(float speed, GrappleGun gun, Transform dummyTransform, float interpolateValue)
    {
        travelSpeed = speed;
        grappleGun = gun;
        returnTransform = dummyTransform;
        retractInterpolateValue = interpolateValue;
    }

    public void FireHook(Vector3 pos, Quaternion rot)
    {
        fired = true;
        retracting = false;

        cd.enabled = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.detectCollisions = true;

        transform.position = pos;
        transform.rotation = rot;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.velocity = travelSpeed * transform.forward;
    }

    public void ReturnHook()
    {
        switch (state)
        {
            case GrappleState.None:
                break;
            case GrappleState.Red:
                grappleGun.StopGrappleRed();
                break;
            case GrappleState.Green:
                grappleGun.StopGrappleGreen();
                break;
            case GrappleState.Blue:
                grappleGun.StopGrappleBlue();
                break;
            case GrappleState.Orange:
                grappleGun.StopGrappleOrange();
                break;
        }
        state = GrappleState.None;
        cd.enabled = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        retracting = true;
    }
    public void ReturnHook(bool instant)
    {
        if (instant)
        {
            FinishRetract();
        }
        else
        {
            ReturnHook();
        }
    }

    private void FinishRetract()
    {
        cd.enabled = true;
        retracting = false;
        fired = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.detectCollisions = true;

        gameObject.SetActive(false);
    }
}