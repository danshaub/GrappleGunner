using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedInteraction : I_GrappleInteraction
{
    private RedOptions props;
    private Transform currentGunTip, currentHookPoint;
    private Rigidbody playerRB;
    private Vector3 ropeDirection;

    private float breakAmount;
    private bool reverse;

    public void OnHit(Transform gunTip, Transform hookPoint)
    {
        props = GrappleManager.Instance.redProperties;
        playerRB = PlayerManager.Instance.movementController.rigidbody;
        currentGunTip = gunTip;
        currentHookPoint = hookPoint;

        PlayerManager.Instance.allowMovement = false;
        PlayerManager.Instance.useGravity = false;
        PlayerManager.Instance.useFriction = false;

    }
    public void OnRelease()
    {
        PlayerManager.Instance.allowMovement = true;
        PlayerManager.Instance.useGravity = true;
        PlayerManager.Instance.useFriction = true;

        playerRB.velocity = Vector3.zero;
    }
    public void OnFixedUpdate()
    {
        ropeDirection = (currentGunTip.position - currentHookPoint.position).normalized;
        float distanceFromPoint = Vector3.Distance(currentGunTip.position, currentHookPoint.position);

        float multiplier = props.redVelocityCurve.Evaluate(distanceFromPoint);
        Vector3 targetVelocity = -ropeDirection * props.redGrappleSpeed * multiplier;
        targetVelocity *= (1 - breakAmount);

        if (reverse)
        {
            targetVelocity *= -1;
        }

        float damper = multiplier >= 1 ? props.redVelocityDamper : 1;
        playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);
        playerRB.AddForce(PlayerManager.Instance.movementController.groundNormal * props.redGroundKick, ForceMode.VelocityChange);

        reverse = false;
    }
    public void OnReelIn(float reelStrength)
    {
        breakAmount = reelStrength;
    }
    public void OnReelOut()
    {
        reverse = true;
    }
    public void OnSwing(Vector3 swingVelocity)
    {
        return;
    }
}
