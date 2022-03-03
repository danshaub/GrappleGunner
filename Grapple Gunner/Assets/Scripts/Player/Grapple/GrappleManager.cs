using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleManager : MonoBehaviour
{
    #region Singleton
    public static GrappleManager _instance;
    private void Awake()
    {
        if (_instance)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public GrappleOptions options;
    public RedProperties redProperties;
    public GreenProperties greenProperties;
    public OrangeProperties orangeProperties;
    public BlueProperties blueProperties;

    [Tooltip("References to the gun objects. Indices 0 and 1 are left and right respectively")]
    public GrappleGun[] guns = new GrappleGun[2];
    [Tooltip("References to the hook objects. Indices 0 and 1 are left and right respectively")]
    public GrappleHook[] hooks = new GrappleHook[2];
    public bool[] reticleIsActive = { true, true };
    public I_GrappleInteraction[] grappleInteractions = new I_GrappleInteraction[2];


    private void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            if (reticleIsActive[i]) guns[i].UpdateReticle();
        }
    }

    public void FireHook(int index)
    {
        hooks[index].FireHook();
    }

    public void ReleaseHook(int index)
    {
        hooks[index].ReleaseHook();
    }

    public void DisableReticle(int index)
    {
        guns[index].DisableReticle();
    }
    public void EnableReticle(int index)
    {
        guns[index].EnableReticle();
    }

    public void BeginGrapple(int index, GrapplePoint.GrappleType type){
        switch(type){
            case GrapplePoint.GrappleType.Red:
                grappleInteractions[index] = new RedInteraction();
                hooks[(index+1)%2].ReleaseHook();
                break;
            case GrapplePoint.GrappleType.Orange:
                grappleInteractions[index] = new OrangeInteraction();
                break;
            case GrapplePoint.GrappleType.Green:
                grappleInteractions[index] = new GreenInteraction();
                break;
            case GrapplePoint.GrappleType.Blue:
                grappleInteractions[index] = new BlueInteraction();
                break;
        }
    }

    public void EndGrapple(int index){
        grappleInteractions[index] = null;
    }

}
