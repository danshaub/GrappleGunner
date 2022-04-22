using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueInteraction : I_GrappleInteraction
{
    private BlueOptions props;
    private Transform currentGunTip;
    private Transform currentHoookPoint;
    public BluePoint bluePoint { get; private set; }
    private Rigidbody pointRB;
    private Rigidbody hookRB;
    private Rigidbody playerRB;
    private int gunIndex;

    public bool attemptedRelease = false;
    private bool launchOnRelease = false;

    private float springDamper;
    public bool blockIsStored { get; private set; } = false;
    private bool buttonRealeased = true;

    public void OnHit(Transform gunTip, Transform hookPoint, GrapplePoint grapplePoint, int index)
    {
        GrappleManager.Instance.guns[index].lightning.SetColor(GrappleManager.Instance.LightningColors.blueColor);
        props = GrappleManager.Instance.blueOptions;

        gunIndex = index;

        playerRB = PlayerManager.Instance.movementController.rigidbody;

        currentGunTip = gunTip;
        currentHoookPoint = hookPoint;
        bluePoint = (BluePoint)grapplePoint;

        if (bluePoint.blockHeld)
        {
            GrappleManager.Instance.ReleaseHook((gunIndex + 1) % 2, true);
        }

        bluePoint.blockHeld = true;


        pointRB = bluePoint.GetComponent<Rigidbody>();
        hookRB = currentHoookPoint.GetComponentInParent<Rigidbody>();

        bluePoint.GetComponent<Collider>().material = props.heldPhysicsMaterial;

        pointRB.useGravity = false;
        hookRB.mass = props.hookMass;

        bluePoint.gameObject.layer = 14;

        // Sets spring damper to critical damp value
        // https://physics.stackexchange.com/questions/191569/damping-a-spring-force
        springDamper = 2 * Mathf.Sqrt(props.hookMass * props.springStrength);
    }
    public void OnRelease()
    {
        bluePoint.GetComponent<Collider>().material = props.releasedPhysicsMaterial;
        bluePoint.gameObject.layer = 13;
        pointRB.useGravity = true;
        hookRB.mass = 0;

        bluePoint.blockHeld = false;

        GrappleManager.Instance.EnableReticle(gunIndex);

        if (launchOnRelease)
        {
            SFXManager.Instance.PlaySFXOneShot("LaunchMagLev");
            bluePoint.ApplyLaunchForce(currentGunTip.forward * props.launchForce);
        }
    }
    public void OnFixedUpdate()
    {
        if (blockIsStored)
        {
            hookRB.MovePosition(currentGunTip.TransformPoint(props.storingTargetHookPosition - currentHoookPoint.localPosition) +
                                Time.fixedDeltaTime * PlayerManager.Instance.movementController.rigidbody.velocity);
            hookRB.MoveRotation(currentGunTip.rotation);
        }
        else
        {
            Vector3 distanceFromTarget = currentHoookPoint.position - currentGunTip.TransformPoint(props.targetHookPosition);

            Vector3 springForce = (props.springStrength * -distanceFromTarget) - (springDamper * (hookRB.velocity - playerRB.velocity));

            if (distanceFromTarget.magnitude > props.velocityClampDistance)
            {
                hookRB.velocity = Vector3.ClampMagnitude(hookRB.velocity, props.maxHookVelocity);
                springForce = Vector3.ClampMagnitude(springForce, props.maxSpringForce);
            }

            if (bluePoint.colliding)
            {
                float forceAgainstCollision = Vector3.Dot(bluePoint.collisionNormal, springForce);
                springForce = Vector3.ProjectOnPlane(springForce, bluePoint.collisionNormal);
                springForce += bluePoint.collisionNormal * Mathf.Clamp(forceAgainstCollision, -props.maxPushbackForce, props.maxPushbackForce);
            }

            hookRB.AddForce(springForce);

            //Find the rotation difference in eulers
            Quaternion diff = Quaternion.Inverse(hookRB.rotation) * currentGunTip.rotation;
            Vector3 eulers = OrientTorque(diff.eulerAngles);
            Vector3 torque = eulers;
            //put the torque back in body space
            torque = hookRB.rotation * torque;

            //just zero out the current angularVelocity so it doesnt interfere
            hookRB.angularVelocity = Vector3.zero;

            hookRB.AddTorque(torque, ForceMode.VelocityChange);
        }
    }

    private Vector3 OrientTorque(Vector3 torque)
    {
        // Quaternion's Euler conversion results in (0-360)
        // For torque, we need -180 to 180.

        return new Vector3
        (
        torque.x > 180f ? torque.x - 360f : torque.x,
        torque.y > 180f ? torque.y - 360f : torque.y,
        torque.z > 180f ? torque.z - 360f : torque.z
        );
    }

    public void OnReelIn(float reelStrength)
    {
        if (bluePoint.storageLockedByCooldown) return;
        if (reelStrength > props.storeBlockInputThreshold)
        {
            if (buttonRealeased)
            {
                if (blockIsStored)
                {
                    SFXManager.Instance.PlaySFXOneShot("ReleaseMagLev");
                    TakeOutBlock(false);
                }
                else
                {
                    StoreBlock();
                }
                buttonRealeased = false;
            }
        }
        else
        {
            buttonRealeased = true;
        }
    }

    private void StoreBlock()
    {
        if (!bluePoint.canStore) return;

        SFXManager.Instance.PlaySFXOneShot("StoreMagLev");
        blockIsStored = true;
        GrappleManager.Instance.grappleLocked[gunIndex] = true;
        bluePoint.ShowMiniPoint(currentHoookPoint, props.miniPointLocalPosition, props.miniPointScale, props.interpolationValue);
        hookRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        hookRB.isKinematic = true;

        bluePoint.gameObject.layer = 15;
    }

    public void TakeOutBlock(bool force)
    {
        if (force || bluePoint.validTakeOutLocation)
        {
            blockIsStored = false;
            GrappleManager.Instance.grappleLocked[gunIndex] = false;
            bluePoint.HideMiniPoint();

            hookRB.isKinematic = false;
            hookRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            bluePoint.gameObject.layer = 14;

            if (attemptedRelease)
            {
                GrappleManager.Instance.ReleaseHook(gunIndex); ;
            }
        }
    }
    public void OnReelOut()
    {
        if (launchOnRelease) return;
        launchOnRelease = true;

        if (blockIsStored)
        {
            if (bluePoint.validTakeOutLocation)
            {
                SFXManager.Instance.PlaySFXOneShot("ReleaseMagLev");
                TakeOutBlock(false);
            }
            else
            {
                launchOnRelease = false;
                return;
            }
        }

        GrappleManager.Instance.ReleaseHook(gunIndex); ;
    }

    // Called from OrangeInteraction TODO: Refactor to be called by TP Manager
    public void TeleportWithBlock(Vector3 playerTargetPosition)
    {
        Vector3 hookOffset = hookRB.position - playerRB.position;
        Vector3 blockOffset = pointRB.position - playerRB.position;

        playerRB.MovePosition(playerTargetPosition);
        hookRB.MovePosition(playerTargetPosition + hookOffset);
        pointRB.MovePosition(playerTargetPosition + blockOffset);
    }

    public void DestroyBlock()
    {
        if (blockIsStored)
        {
            TakeOutBlock(true);
        }

        GrappleManager.Instance.ReleaseHook(gunIndex);

        bluePoint.DestroyBlock();
    }
    public void OnSwing(Vector3 swingVelocity)
    {
        return;
    }
}