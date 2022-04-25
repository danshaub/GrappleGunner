using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeInteraction : I_GrappleInteraction
{
    private OrangeOptions props;
    private OrangePoint orangePoint;
    private Rigidbody playerRB;
    private Rigidbody playerTargetRB;
    private Rigidbody blockRB;
    private Rigidbody blockTargetRB;
    private bool teleporting = false;
    private int gunIndex;
    public void OnHit(Transform gunTip, Transform hookPoint, GrapplePoint grapplePoint, int index)
    {
        gunIndex = index;
        orangePoint = (OrangePoint)grapplePoint;

        if(!orangePoint.active){
            GrappleManager.Instance.ReleaseHook(gunIndex);
            return;
        }

        GrappleManager.Instance.guns[index].lightning.SetColor(GrappleManager.Instance.LightningColors.orangeColor);
        props = GrappleManager.Instance.orangeOptions;


        playerRB = PlayerManager.Instance.movementController.rigidbody;
        playerTargetRB = orangePoint.playerTargetRigidbody;
        blockRB = orangePoint.blockRigidbody;
        blockTargetRB = orangePoint.blockTargetRigidbody;
    }
    public void OnRelease()
    {
        return;
    }
    public void OnFixedUpdate()
    {
        if (!orangePoint.active)
        {
            GrappleManager.Instance.ReleaseHook(gunIndex);
        }
    }
    public void OnReelIn(float reelStrength)
    {
        if (!teleporting && reelStrength > props.reelInThreshold)
        {
            QueueTeleport();
        }
    }
    public void OnReelOut()
    {
        if (!teleporting)
        {
            QueueTeleport();
        }
    }
    public void OnSwing(Vector3 swingVelocity)
    {
        return;
    }

    private void QueueTeleport()
    {
        teleporting = true;

        GrappleManager.Instance.QueueTeleport(this, gunIndex);

        VFXManager.Instance.transitionSystem.SetParticleColor(props.particleColor);
        VFXManager.Instance.transitionSystem.StartTransition();
    }

    public void Teleport()
    {
        orangePoint.DecrementUses();

        Vector3 playerTargetPosition = playerTargetRB.transform.position;
        Vector3 playerTargetVelocity = blockRB != null ? blockRB.velocity : Vector3.zero;

        Vector3 blockTargetPosition = blockTargetRB.transform.position;
        Vector3 blockTargetVelocity = blockRB != null ? playerRB.velocity : Vector3.zero;

        

        if(GrappleManager.Instance.grappleInteractions[(gunIndex + 1) % 2]?.GetType() == typeof(BlueInteraction)){
            ((BlueInteraction)GrappleManager.Instance.grappleInteractions[(gunIndex + 1) % 2]).TeleportWithBlock(playerTargetPosition);
        }
        else{
            playerRB.MovePosition(playerTargetPosition);
        }

        playerRB.velocity = playerTargetVelocity;

        if (blockRB != null)
        {
            blockRB.MovePosition(blockTargetPosition);
            blockRB.velocity = blockTargetVelocity;
        }
        else
        {
            orangePoint.transform.position = blockTargetPosition;
        }

        GrappleManager.Instance.grappleLocked[gunIndex] = false;

        GrappleManager.Instance.ReleaseHook(gunIndex, true);

        teleporting = false;

        VFXManager.Instance.transitionSystem.EndTransition();
    }
}
