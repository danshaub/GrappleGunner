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
        GrappleManager.Instance.guns[index].lightening.SetColor(GrappleManager.Instance.LighteningColors.orangeColor);
        props = GrappleManager.Instance.orangeOptions;

        orangePoint = (OrangePoint)grapplePoint;

        playerRB = PlayerManager.Instance.movementController.rigidbody;
        playerTargetRB = orangePoint.playerTargetRigidbody;
        blockRB = orangePoint.blockRigidbody;
        blockTargetRB = orangePoint.blockTargetRigidbody;

        gunIndex = index;

        return;
    }
    public void OnRelease()
    {
        return;
    }
    public void OnFixedUpdate()
    {
        return;
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
        // TODO: Start transition effect here
    }

    public void Teleport()
    {
        orangePoint.DecrementUses();

        Vector3 playerTargetPosition = playerTargetRB.transform.position;
        Vector3 playerTargetVelocity = blockRB != null ? blockRB.velocity : Vector3.zero;

        Vector3 blockTargetPosition = blockTargetRB.transform.position;
        Vector3 blockTargetVelocity = blockRB != null ? playerRB.velocity : Vector3.zero;

        playerRB.MovePosition(playerTargetPosition);
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

        GrappleManager.Instance.grappleLocked = false;

        GrappleManager.Instance.ReleaseHook(0);
        GrappleManager.Instance.ReleaseHook(1);

        teleporting = false;
    }
}
