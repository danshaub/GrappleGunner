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
        GrappleManager.Instance.guns[index].lightning.SetColor(GrappleManager.Instance.LightningColors.greenColor);

        props = GrappleManager.Instance.greenOptions;
        playerRB = PlayerManager.Instance.movementController.rigidbody;
        currentGunTip = gunTip;
        currentHookPoint = hookPoint;
        currentIndex = index;

        joint = PlayerManager.Instance.player.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = PlayerManager.Instance.player.transform.InverseTransformPoint(gunTip.position);
        // joint.anchor = player.InverseTransformPoint(vrCamera.position);
        joint.connectedAnchor = currentHookPoint.position;

        float distanceFromPoint = Vector3.Distance(gunTip.position, currentHookPoint.position);

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = distanceFromPoint;
        limit.bounciness = 0;
        limit.contactDistance = props.limitContactDistance;
        joint.linearLimit = limit;

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

        if (reeling && distanceFromPoint > PlayerManager.Instance.playerHeight * props.snapDistanceMultiplier)
        {
            playerRB.AddForce(-ropeDirection * reelInput * props.greenReelForce);
            if (PlayerManager.Instance.grounded)
            {
                playerRB.AddForce(-ropeDirection * props.groundedReelMultiplier);
            }
            reeledThisFrame = true;
        }

        if (PlayerManager.Instance.grounded)
        {
            groundedThisFrame = true;

            limit.limit = distanceFromPoint;

            greenCurrentSpoolSpeed = 0f;

            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
            // joint.xMotion = ConfigurableJointMotion.Limited;
            // joint.yMotion = ConfigurableJointMotion.Limited;
            // joint.zMotion = ConfigurableJointMotion.Limited;
        }
        else if (slacking)
        {
            slackedThisFrame = true;
            greenCurrentSpoolSpeed = Mathf.Lerp(greenCurrentSpoolSpeed, props.greenMaxSlackSpeed, props.greenSlackDamper);
            limit.limit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
        }
        else if (distanceFromPoint <= PlayerManager.Instance.playerHeight * props.snapDistanceMultiplier)
        {
            float multiplier = props.greenSnapVelocityCurve.Evaluate(distanceFromPoint);
            Vector3 targetVelocity = -ropeDirection * props.greenSnapSpeed * multiplier;

            float damper = multiplier >= 1 ? props.greenSnapVelocityDamper : 1;
            playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);
        }
        else
        {
            // allow auto retracting, but not slacking
            limit.limit = Mathf.Clamp(distanceFromPoint, PlayerManager.Instance.playerHeight - .05f, joint.linearLimit.limit);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;

            playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, Vector3.zero, props.swingDamper);
        }

        if (!reeledThisFrame && !slackedThisFrame)
        {
            greenCurrentSpoolSpeed = Mathf.Lerp(greenCurrentSpoolSpeed, 0, props.greenSlackDamper);

            if (!groundedThisFrame && greenCurrentSpoolSpeed >= 0.001)
            {
                limit.limit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);
            }
        }

        joint.linearLimit = limit;

        slacking = false;
    }
    public void OnReelIn(float reelStrength)
    {
        reelInput = reelStrength;
        reeling = reelInput > props.reelDeadZone;
    }
    public void OnReelOut()
    {
        slacking = true;
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
        if (GrappleManager.Instance.grappleInteractions[(currentIndex + 1) % 2]?.GetType() == typeof(GreenInteraction))
        {
            return props.doubleGreenMultiplier;
        }
        else
        {
            return props.swingForceMultiplier;
        }
    }
}
