using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public bool fired { get; private set; }
    public bool retracting { get; private set; }

    private Collider cd;
    private Rigidbody rb;
    public Transform dummyHookTransform;
    public GrapplePoint grapplePoint { get; private set; }

    private FixedJoint joint;
    [Tooltip("Set to 0 for left, set to 1 for right")]
    public int index;

    void Start()
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
            transform.position = Vector3.Lerp(transform.position, dummyHookTransform.position, GrappleManager.Instance.options.retractInterpolateValue);
            transform.rotation = Quaternion.Slerp(transform.rotation, dummyHookTransform.rotation, GrappleManager.Instance.options.retractInterpolateValue);

            if (Vector3.Distance(transform.position, dummyHookTransform.position) <= GrappleManager.Instance.options.returnSnapDistance)
            {
                FinishRetract();
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        cd.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;

        if (other.gameObject.CompareTag("Hookable"))
        {

            grapplePoint = other.gameObject.GetComponent<GrapplePoint>();
            grapplePoint.OnPointHit();

            if (grapplePoint.useRaycastPosition)
            {
                transform.position = other.contacts[0].point;
                transform.forward = (-other.contacts[0].normal);
            }
            else
            {
                transform.position = grapplePoint.GetGrapplePosition();
                transform.rotation = grapplePoint.GetGrappleRotation();
            }

            GrapplePoint.GrappleType[] interactablePoints =
                {GrapplePoint.GrappleType.Red,
                 GrapplePoint.GrappleType.Orange,
                 GrapplePoint.GrappleType.Green,
                 GrapplePoint.GrappleType.Blue};


            if (Array.IndexOf(interactablePoints, grapplePoint.type) >= 0)
            {
                GrappleManager.Instance.BeginGrapple(index, grapplePoint.type);
            }
            else
            {
                Invoke("ReleaseHook", GrappleManager.Instance.options.timeBeforeRetract);
            }
        }
        else
        {

            transform.position = other.contacts[0].point;
            transform.rotation = Quaternion.LookRotation(-other.contacts[0].normal, Vector3.up);
            Invoke("ReleaseHook", GrappleManager.Instance.options.timeBeforeRetract);
        }

        Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();
        if (otherRB)
        {
            rb.constraints = RigidbodyConstraints.None;
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = other.gameObject.GetComponent<Rigidbody>();
        }

    }

    public void FireHook()
    {
        gameObject.SetActive(true);
        dummyHookTransform.gameObject.SetActive(false);

        fired = true;
        retracting = false;

        cd.enabled = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.detectCollisions = true;

        transform.position = dummyHookTransform.position;
        transform.rotation = dummyHookTransform.rotation;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.velocity = GrappleManager.Instance.options.hookTravelSpeed * transform.forward;
    }

    public void ReleaseHook()
    {
        if (grapplePoint)
        {
            grapplePoint.OnPointReleased();
            grapplePoint = null;
        }
        ReleaseHook(false);
    }

    public void ReleaseHook(bool instant)
    {
        GrappleManager.Instance.EndGrapple(index);
        Destroy(joint);
        if (instant)
        {
            FinishRetract();
        }
        else
        {
            cd.enabled = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
            rb.detectCollisions = false;
            retracting = true;
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

        dummyHookTransform.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
