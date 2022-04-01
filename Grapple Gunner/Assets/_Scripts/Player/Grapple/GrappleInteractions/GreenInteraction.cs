using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenInteraction : I_GrappleInteraction
{
    private static int numGreenHooks = 0;
    private GreenOptions props;
    private Rigidbody playerRB;
    private Rigidbody pointRB;
    private Transform currentGunTip;
    private Transform currentHookPoint;
    private int gunIndex;
    private float reelInput;
    private bool reelingIn;
    private bool reelingOut;
    private float distanceFromPoint;
    private GreenPoint greenPoint;
    private SoftJointLimit linearLimit;
    private SoftJointLimitSpring springLimit;

    private ConfigurableJoint joint;
    private Vector3 ropeDirection;

    public void OnHit(Transform gunTip, Transform hookPoint, GrapplePoint grapplePoint, int index)
    {
        numGreenHooks++;

        GrappleManager.Instance.guns[index].lightning.SetColor(GrappleManager.Instance.LightningColors.greenColor);

        props = GrappleManager.Instance.greenOptions;
        playerRB = PlayerManager.Instance.movementController.rigidbody;
        currentGunTip = gunTip;
        currentHookPoint = hookPoint;
        gunIndex = index;

        greenPoint = (GreenPoint)grapplePoint;

        joint = PlayerManager.Instance.player.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = PlayerManager.Instance.player.transform.InverseTransformPoint(gunTip.position);

        joint.connectedAnchor = currentHookPoint.position;

        distanceFromPoint = Vector3.Distance(gunTip.position, currentHookPoint.position);

        linearLimit = new SoftJointLimit();
        linearLimit.bounciness = 0;
        linearLimit.contactDistance = 0;
        linearLimit.limit = distanceFromPoint;
        joint.linearLimit = linearLimit;

        springLimit = new SoftJointLimitSpring();
        springLimit.damper = 0;
        springLimit.spring = 0;
        joint.linearLimitSpring = springLimit;

        pointRB = grapplePoint.GetComponent<Rigidbody>();
    }
    public void OnRelease()
    {
        numGreenHooks--;
        PlayerManager.Instance.useGrapplePhysicsMaterial = false;
        GrappleManager.Instance.groundChecks[gunIndex] = true;
        Object.Destroy(joint);
    }
    public void OnFixedUpdate()
    {
        GrappleManager.Instance.groundChecks[gunIndex] = true;

        PlayerManager.Instance.useGrapplePhysicsMaterial = true;

        joint.connectedAnchor = currentHookPoint.position;
        joint.anchor = PlayerManager.Instance.player.transform.InverseTransformPoint(currentGunTip.position);

        ropeDirection = (currentGunTip.position - currentHookPoint.position).normalized;
        distanceFromPoint = Vector3.Distance(currentGunTip.position, currentHookPoint.position);

        springLimit.damper = 0;
        springLimit.spring = 0;
        linearLimit.limit = joint.linearLimit.limit;

        springLimit.damper = props.springForce;
        springLimit.spring = props.springDamper;

        if (reelingIn)
        {
            playerRB.AddForce(reelInput * props.reelInForce * ForceMultiplier() * (-ropeDirection));
            playerRB.velocity = Vector3.Lerp(playerRB.velocity, Vector3.zero, props.reelInDistanceVelocityDamper.Evaluate(distanceFromPoint));
        }

        if(greenPoint.playerCollided){
            playerRB.velocity = Vector3.Lerp(playerRB.velocity, Vector3.zero, props.contactVelocityDamper);
        }

        if (PlayerManager.Instance.grounded || reelingOut)
        {
            linearLimit.limit = distanceFromPoint;

            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
        }
        else
        {
            // allow auto retracting, but not slacking
            linearLimit.limit = Mathf.Clamp(distanceFromPoint, props.minLinearLimit, joint.linearLimit.limit);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
        }

        joint.linearLimit = linearLimit;
        joint.linearLimitSpring = springLimit;

        reelingOut = false;
    }
    public void OnReelIn(float reelStrength)
    {
        reelInput = reelStrength;
        reelingIn = reelInput > props.reelInDeadZone;

        if(reelingIn){
            GrappleManager.Instance.groundChecks[gunIndex] = false;
        }
    }
    public void OnReelOut()
    {
        reelingOut = true;
    }
    public void OnSwing(Vector3 swingVelocity)
    {
        swingVelocity = PlayerManager.Instance.player.transform.TransformVector(swingVelocity);

        float swingMagnitude = Vector3.Dot(swingVelocity, ropeDirection);
        if (swingMagnitude > props.swingInputThreshold)
        {
            GrappleManager.Instance.groundChecks[gunIndex] = false;
            swingMagnitude = Mathf.Clamp(swingMagnitude, props.swingInputThreshold, props.maxSwingVelocity);
            playerRB.AddForce(swingMagnitude * props.swingForce * ForceMultiplier() * DoubleGreenMultiplier() * (-ropeDirection));
        }
    }

    public float DoubleGreenMultiplier(){
        return (numGreenHooks == 2 ? props.doubleGreenMultiplier : 1);
    }
    public float ForceMultiplier(){
        return (PlayerManager.Instance.grounded ? props.forceGroundedMultiplier : 1) * props.forceDistanceMultiplier.Evaluate(distanceFromPoint);
    }
}
