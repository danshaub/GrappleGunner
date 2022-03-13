using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedInteraction : I_GrappleInteraction
{
    private RedOptions props;
    private Transform currentGunTip, currentHookPoint;
    private Rigidbody playerRB;
    private Rigidbody pointRB;
    private Vector3 ropeDirection;

    private float speedIncreaseInput;
    private bool brake;

    public void OnHit(Transform gunTip, Transform hookPoint, GrapplePoint grapplePoint, int index)
    {
        GrappleManager.Instance.guns[index].lightning.SetColor(GrappleManager.Instance.LightningColors.redColor);
        props = GrappleManager.Instance.redOptions;
        playerRB = PlayerManager.Instance.movementController.rigidbody;
        currentGunTip = gunTip;
        currentHookPoint = hookPoint;

        PlayerManager.Instance.allowMovement = false;
        PlayerManager.Instance.useGravity = false;
        PlayerManager.Instance.useFriction = false;

        pointRB = grapplePoint.GetComponent<Rigidbody>();

    }
    public void OnRelease()
    {
        PlayerManager.Instance.allowMovement = true;
        PlayerManager.Instance.useGravity = true;
        PlayerManager.Instance.useFriction = true;
        PlayerManager.Instance.useGrapplePhysicsMaterial = false;

        playerRB.velocity = Vector3.zero;
    }
    public void OnFixedUpdate()
    {
        PlayerManager.Instance.useGrapplePhysicsMaterial = true;
        ropeDirection = (currentGunTip.position - currentHookPoint.position).normalized;
        float distanceFromPoint = Vector3.Distance(currentGunTip.position, currentHookPoint.position);

        float multiplier = props.redVelocityCurve.Evaluate(distanceFromPoint);
        Vector3 targetVelocity = -ropeDirection * props.redGrappleSpeed * multiplier;
        if(multiplier >= 1){
            targetVelocity *= (props.speedIncreaseMultiplier * speedIncreaseInput) + (1 - speedIncreaseInput);
        }

        if (brake)
        {
            targetVelocity = Vector3.zero;
        }

        if(pointRB != null){
            targetVelocity += pointRB.velocity;
        }

        float damper = multiplier >= 1 ? props.redVelocityDamper : 1;
        damper = brake ? 1 : damper;
        playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);
        playerRB.AddForce(PlayerManager.Instance.movementController.groundNormal * props.redGroundKick, ForceMode.VelocityChange);

        brake = false;
    }
    public void OnReelIn(float reelStrength)
    {
        speedIncreaseInput = reelStrength;
    }
    public void OnReelOut()
    {
        brake = true;
    }
    public void OnSwing(Vector3 swingVelocity)
    {
        return;
    }
}
