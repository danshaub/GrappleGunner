using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenInteraction : I_GrappleInteraction
{
    private GreenOptions props;
    private Rigidbody playerRB;
    private Rigidbody pointRB;
    private Transform currentGunTip;
    private Transform currentHookPoint;
    private int currentIndex;
    private float reelInput;
    private bool reeling;
    private bool slacking;
    private float greenCurrentSpoolSpeed;

    private ConfigurableJoint joint;
    private Vector3 ropeDirection;

    public void OnHit(Transform gunTip, Transform hookPoint, GrapplePoint grapplePoint, int index)
    {
        GrappleManager.Instance.guns[index].lightening.SetColor(GrappleManager.Instance.LighteningColors.greenColor);

        props = GrappleManager.Instance.greenOptions;
        playerRB = PlayerManager.Instance.movementController.rigidbody;
        currentGunTip = gunTip;
        currentHookPoint = hookPoint;
        currentIndex = index;

        joint = PlayerManager.Instance.player.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;

        pointRB = grapplePoint.GetComponent<Rigidbody>();
    }
    public void OnRelease()
    {
        PlayerManager.Instance.useGrapplePhysicsMaterial = false;
        Object.Destroy(joint);
    }
    public void OnFixedUpdate()
    {
        PlayerManager.Instance.useGrapplePhysicsMaterial = true;

        joint.connectedAnchor = currentHookPoint.position;
        joint.anchor = PlayerManager.Instance.player.transform.InverseTransformPoint(currentGunTip.position);

        ropeDirection = (currentGunTip.position - currentHookPoint.position).normalized;
        float distanceFromPoint = Vector3.Distance(currentGunTip.position, currentHookPoint.position);

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = joint.linearLimit.limit;
        limit.bounciness = 0;
        limit.contactDistance = props.limitContactDistance;

        bool slackedThisFrame = false;
        bool reeledThisFrame = false;
        bool groundedThisFrame = false;

        if (reeling)
        {
            playerRB.AddForce(-ropeDirection * reelInput * props.greenReelForce);
            if (PlayerManager.Instance.grounded)
            {
                Debug.Log("JFKSDJF");
                playerRB.AddForce(-ropeDirection * props.groundedReelMultiplier);
            }
        }

        reeling = false;
    }
    public void OnReelIn(float reelStrength)
    {
        reelInput = reelStrength;
        reeling = reelInput > props.reelDeadZone;
    }
    public void OnReelOut()
    {
        reeling = true;
    }
    public void OnSwing(Vector3 swingVelocity)
    {
        swingVelocity = PlayerManager.Instance.player.transform.TransformVector(swingVelocity);

        float swingMagnitude = Vector3.Dot(swingVelocity, ropeDirection);
        swingMagnitude = Mathf.Clamp(swingMagnitude, props.swingVelocityThreshold, props.maxSwingVelocity);
        if (swingMagnitude > props.swingVelocityThreshold)
        {
            playerRB.AddForce(-ropeDirection * swingMagnitude * SwingForceMultiplier());
        }
    }

    public float SwingForceMultiplier()
    {
        if(GrappleManager.Instance.grappleInteractions[(currentIndex + 1) % 2]?.GetType() == typeof(GreenInteraction)){
            return props.doubleGreenMultiplier;
        }
        else{
            return props.swingForceMultiplier;
        }
    }
}
