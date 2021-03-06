using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleManager : Singleton<GrappleManager>
{
    public GrappleOptions options;
    public RedOptions redOptions;
    public GreenOptions greenOptions;
    public OrangeOptions orangeOptions;
    public BlueOptions blueOptions;

    public LightningOptions LightningOptions;
    public LightningColors LightningColors;

    [Tooltip("References to the gun objects. Indices 0 and 1 are left and right respectively")]
    public GrappleGun[] guns = new GrappleGun[2];
    [Tooltip("References to the hook objects. Indices 0 and 1 are left and right respectively")]
    public GrappleHook[] hooks = new GrappleHook[2];
    public I_GrappleInteraction[] grappleInteractions = new I_GrappleInteraction[2];

    public bool redConnected = false;
    public bool[] groundChecks { get; set; }
    public bool groundCheck
    {
        get
        {
            foreach (bool check in groundChecks)
            {
                if (!check) return false;
            }
            return true;
        }
    }
    public bool[] grappleLocked;
    private bool grappleDisabled;

    private void Start()
    {
        grappleLocked = new bool[] { false, false };
        groundChecks = new bool[] { true, true };
        grappleDisabled = false;
    }

    private void LateUpdate()
    {
        for (int index = 0; index < 2; index++)
        {
            guns[index].UpdateReticle();
            guns[index].DrawRope();

            if (index == 0)
            {
                float targetVolume = Mathf.Lerp(SFXManager.Instance.GetSFXPitch("GunRopeHumRight"),options.ropeHumPitch.Evaluate(guns[index].relativeHookSpeed), options.ropeHumPitchDamper);
                SFXManager.Instance.SetSFXPitch("GunRopeHumRight", targetVolume);
            }
            else
            {
                float targetVolume = Mathf.Lerp(SFXManager.Instance.GetSFXPitch("GunRopeHumLeft"),options.ropeHumPitch.Evaluate(guns[index].relativeHookSpeed), options.ropeHumPitchDamper);
                SFXManager.Instance.SetSFXPitch("GunRopeHumLeft", targetVolume);
            }
        }
    }

    public void FireHook(int index)
    {
        if (grappleDisabled) return;

        if (!grappleLocked[index])
        {
            guns[index].DisableReticle();
            hooks[index].FireHook();

            SFXManager.Instance.PlaySFX("GunFire");
            if (index == 0)
            {
                SFXManager.Instance.SetSFXVolume("GunRopeHumRight", 0.75f, true);
            }
            else
            {
                SFXManager.Instance.SetSFXVolume("GunRopeHumLeft", 0.75f, true);
            }
        }
        else if (grappleInteractions[index]?.GetType() == typeof(BlueInteraction))
        {
            ((BlueInteraction)grappleInteractions[index]).attemptedRelease = false;
        }
    }

    public void ReleaseHook(int index, bool instant)
    {
        if (grappleDisabled) return;

        if (!grappleLocked[index])
        {
            guns[index].EnableReticle();
            hooks[index].ReleaseHook(instant);
        }
        else if (grappleInteractions[index]?.GetType() == typeof(BlueInteraction))
        {
            ((BlueInteraction)grappleInteractions[index]).attemptedRelease = true;
        }
    }

    public void ReleaseHook(int index)
    {
        ReleaseHook(index, false);
    }

    public void SetGrappleDisabled(bool isDisabled)
    {
        if (!grappleDisabled && isDisabled)
        {
            ForceReleaseHook();
            DisableReticle(0);
            DisableReticle(1);
        }
        if (grappleDisabled && !isDisabled)
        {
            EnableReticle(0);
            EnableReticle(1);
        }
        grappleDisabled = isDisabled;
    }

    public void ForceReleaseHook()
    {
        for (int i = 0; i < 2; i++)
        {
            if (grappleInteractions[i]?.GetType() == typeof(BlueInteraction))
            {
                ((BlueInteraction)grappleInteractions[i]).TakeOutBlock(true);
            }
            guns[i].EnableReticle();
            hooks[i].ReleaseHook(true);
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
                    ReleaseHook((index + 1) % 2);
                }
                break;
            case GrapplePoint.GrappleType.Orange:
                grappleInteractions[index] = new OrangeInteraction();
                break;
            case GrapplePoint.GrappleType.Green:
                grappleInteractions[index] = new GreenInteraction();
                if (grappleInteractions[(index + 1) % 2]?.GetType() == typeof(RedInteraction))
                {
                    ReleaseHook((index + 1) % 2);
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

    public void QueueTeleport(OrangeInteraction orangeInteraction, int index)
    {
        if ((grappleInteractions[(index + 1) % 2]?.GetType() == typeof(BlueInteraction) &&
            ((BlueInteraction)grappleInteractions[(index + 1) % 2]).blockIsStored))
        {
        }
        else
        {
            ReleaseHook((index + 1) % 2, true);
        }
        grappleLocked[index] = true;

        StartCoroutine(WaitForTeleportDelay(orangeInteraction));
    }

    private IEnumerator WaitForTeleportDelay(OrangeInteraction orangeInteraction)
    {
        yield return new WaitForSeconds(orangeOptions.teleportTransitionTime);

        orangeInteraction.Teleport();
    }
}
