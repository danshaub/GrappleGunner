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

    public void OnHit(Transform gunTip, Transform hookPoint){
        props = GrappleManager._instance.redProperties;
        playerRB = PlayerManager._instance.movementController.rigidbody;
        currentGunTip = gunTip;
        currentHookPoint = hookPoint;

        PlayerManager._instance.allowMovement = false;
        PlayerManager._instance.useGravity = false;
        PlayerManager._instance.useFriction = false;

    }
    public void OnRelease(){
        PlayerManager._instance.allowMovement = true;
        PlayerManager._instance.useGravity = true;
        PlayerManager._instance.useFriction = true;

        playerRB.velocity = Vector3.zero;
    }
    public void OnFixedUpdate(){
        ropeDirection = (currentGunTip.position - currentHookPoint.position).normalized;
        float distanceFromPoint = Vector3.Distance(currentGunTip.position, currentHookPoint.position);

        float multiplier = props.redVelocityCurve.Evaluate(distanceFromPoint);
        Vector3 targetVelocity = -ropeDirection * props.redGrappleSpeed * multiplier;
        targetVelocity *= (1-breakAmount);

        if(reverse){
            targetVelocity *= -1;
        }
        
        float damper = multiplier >= 1 ? props.redVelocityDamper : 1;
        playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);


        reverse = false;
    }
    public void OnReelIn(float reelStrength){
        breakAmount = reelStrength;
    }
    public void OnReelOut(){
        reverse = true;
    }
    public void OnSwing(Vector3 swingVelocity){
        return;
    }
}
