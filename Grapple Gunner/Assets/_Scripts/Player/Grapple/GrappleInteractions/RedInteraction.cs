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
    private bool wasBraking;

    public void OnHit(Transform gunTip, Transform hookPoint, GrapplePoint grapplePoint, int index)
    {
        wasBraking = false;
        GrappleManager.Instance.guns[index].lightning.SetColor(GrappleManager.Instance.LightningColors.redColor);
        props = GrappleManager.Instance.redOptions;
        playerRB = PlayerManager.Instance.movementController.rigidbody;
        currentGunTip = gunTip;
        currentHookPoint = hookPoint;

        PlayerManager.Instance.allowMovement = false;
        PlayerManager.Instance.useGravity = false;
        PlayerManager.Instance.useFriction = false;

        pointRB = grapplePoint.GetComponent<Rigidbody>();

        GrappleManager.Instance.redConnected = true;

    }
    public void OnRelease()
    {
        PlayerManager.Instance.allowMovement = true;
        PlayerManager.Instance.useGravity = true;
        PlayerManager.Instance.useFriction = true;
        PlayerManager.Instance.useGrapplePhysicsMaterial = false;

        playerRB.velocity = Vector3.zero;

        GrappleManager.Instance.redConnected = false;
    }
    public void OnFixedUpdate()
    {
        PlayerManager.Instance.useGrapplePhysicsMaterial = true;
        PlayerManager.Instance.allowMovement = false;
        ropeDirection = (currentGunTip.position - currentHookPoint.position).normalized;
        float distanceFromPoint = Vector3.Distance(currentGunTip.position, currentHookPoint.position);

        float multiplier = props.velocityCurve.Evaluate(distanceFromPoint);
        Vector3 targetVelocity = -ropeDirection * props.grappleSpeed * multiplier;
        if (multiplier >= 1)
        {
            targetVelocity *= (props.speedIncreaseMultiplier * speedIncreaseInput) + (1 - speedIncreaseInput);
        }

        if (brake)
        {
            if(!wasBraking){
                SFXManager.Instance.PlaySFXOneShot("GearshotBrake");
            }
            targetVelocity = Vector3.zero;
        }

        if(wasBraking && !brake){
            SFXManager.Instance.PlaySFXOneShot("GearshotUnbrake");
        }

        wasBraking = brake;

        if (pointRB != null)
        {
            targetVelocity += pointRB.velocity;
        }

        float damper;
        if (multiplier < 1)
        {
            damper = 1;
        }
        else if (brake)
        {
            damper = props.brakeDamper;
        }
        else
        {
            damper = props.velocityDamper;
        }

        playerRB.velocity = Vector3.Lerp(playerRB.velocity, targetVelocity, damper);
        // playerRB.AddForce(PlayerManager.Instance.movementController.groundNormal * props.groundKick, ForceMode.VelocityChange);

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
