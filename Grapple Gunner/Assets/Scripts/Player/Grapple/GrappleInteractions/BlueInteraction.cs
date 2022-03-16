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

        if (GrappleManager.Instance.grappleInteractions[(gunIndex + 1) % 2]?.GetType() == typeof(BlueInteraction))
        {
            BluePoint otherPoint = ((BlueInteraction)GrappleManager.Instance.grappleInteractions[(gunIndex + 1) % 2]).bluePoint;

            if (bluePoint.Equals(otherPoint))
            {
                GrappleManager.Instance.hooks[(gunIndex + 1) % 2].ReleaseHook();
            }
        }


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

        GrappleManager.Instance.EnableReticle(gunIndex);

        if (launchOnRelease)
        {
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
            hookRB.AddForce(springForce);
            if (distanceFromTarget.magnitude > props.velocityClampDistance)
            {
                hookRB.velocity = Vector3.ClampMagnitude(hookRB.velocity, props.maxHookVelocity);
            }

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
        if (reelStrength > props.storeBlockInputThreshold)
        {
            if (buttonRealeased)
            {
                if (blockIsStored)
                {
                    TakeOutBlock();
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
        blockIsStored = true;
        GrappleManager.Instance.grappleLocked[gunIndex] = true;
        bluePoint.ShowMiniPoint(currentHoookPoint, props.miniPointLocalPosition, props.miniPointScale, props.interpolationValue);
        hookRB.isKinematic = true;
        hookRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        bluePoint.gameObject.layer = 15;
    }

    private void TakeOutBlock()
    {
        if (!bluePoint.validTakeOutLocation) return;
        blockIsStored = false;
        GrappleManager.Instance.grappleLocked[gunIndex] = false;
        bluePoint.HideMiniPoint();

        hookRB.isKinematic = false;
        hookRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        bluePoint.gameObject.layer = 14;

        if (attemptedRelease)
        {
            GrappleManager.Instance.hooks[gunIndex].ReleaseHook();
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
                TakeOutBlock();
            }
            else{
                launchOnRelease = false;
                return;
            }
        }

        GrappleManager.Instance.hooks[gunIndex].ReleaseHook();
    }
    public void OnSwing(Vector3 swingVelocity)
    {
        // Debug.Log("Blue R_Swing");
    }
}