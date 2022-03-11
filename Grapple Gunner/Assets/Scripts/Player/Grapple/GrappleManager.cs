using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleManager : Singleton<GrappleManager>
{
    public GrappleOptions options;
    public RedOptions redProperties;
    public GreenOptions greenProperties;
    public OrangeOptions orangeProperties;
    public BlueOptions blueProperties;

    [Tooltip("References to the gun objects. Indices 0 and 1 are left and right respectively")]
    public GrappleGun[] guns = new GrappleGun[2];
    [Tooltip("References to the hook objects. Indices 0 and 1 are left and right respectively")]
    public GrappleHook[] hooks = new GrappleHook[2];
    public I_GrappleInteraction[] grappleInteractions = new I_GrappleInteraction[2];

    [HideInInspector] public bool grappleLocked = false;


    private void LateUpdate()
    {
        for (int index = 0; index < 2; index++)
        {
            guns[index].UpdateReticle();
            guns[index].DrawRope();
        }
    }

    public void FireHook(int index)
    {
        if (!grappleLocked)
        {
            guns[index].DisableReticle();
            hooks[index].FireHook();
        }
    }

    public void ReleaseHook(int index)
    {
        if (!grappleLocked)
        {
            guns[index].EnableReticle();
            hooks[index].ReleaseHook();
        }
    }

    public void DisableReticle(int index)
    {
        guns[index].DisableReticle();
    }
    public void EnableReticle(int index)
    {
        guns[index].EnableReticle();
    }

    public void BeginGrapple(int index, GrapplePoint.GrappleType type)
    {

        switch (type)
        {
            case GrapplePoint.GrappleType.Red:
                grappleInteractions[index] = new RedInteraction();
                if (grappleInteractions[(index + 1) % 2]?.GetType() == typeof(RedInteraction) ||
                    grappleInteractions[(index + 1) % 2]?.GetType() == typeof(GreenInteraction))
                {
                    hooks[(index + 1) % 2]?.ReleaseHook();
                    guns[(index + 1) % 2].EnableReticle();
                }
                break;
            case GrapplePoint.GrappleType.Orange:
                grappleInteractions[index] = new OrangeInteraction();
                break;
            case GrapplePoint.GrappleType.Green:
                grappleInteractions[index] = new GreenInteraction();
                if (grappleInteractions[(index + 1) % 2]?.GetType() == typeof(RedInteraction))
                {
                    hooks[(index + 1) % 2]?.ReleaseHook();
                    guns[(index + 1) % 2].EnableReticle();
                }
                break;
            case GrapplePoint.GrappleType.Blue:
                grappleInteractions[index] = new BlueInteraction();
                break;
        }

        if (grappleInteractions[index] != null)
        {
            grappleInteractions[index].OnHit(guns[index].gunTip, guns[index].hookPoint, hooks[index].grapplePoint, index);
        }
    }

    public void EndGrapple(int index)
    {
        if (grappleInteractions[index] != null)
        {
            grappleInteractions[index].OnRelease();
        }

        grappleInteractions[index] = null;
    }

    public void QueueTeleport(OrangeInteraction orangeInteraction, int index){
        hooks[(index + 1) % 2]?.ReleaseHook(true);
        grappleLocked = true;

        StartCoroutine(WaitForTeleportDelay(orangeInteraction));
    }

    private IEnumerator WaitForTeleportDelay(OrangeInteraction orangeInteraction){
        yield return new WaitForSeconds(orangeProperties.teleportTransitionTime);

        orangeInteraction.Teleport();
    }
}
