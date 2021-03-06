using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles calling of all grapple interactions while grappled
public class PlayerGrappleController : MonoBehaviour
{
    private bool[] reelingIn = { false, false };
    private bool[] reelingOut = { false, false };
    // Velocity of controller in XR rig local space
    private Vector3[] swingVelocity = new Vector3[2];
    private float[] reelInInput = new float[2];

    private void FixedUpdate()
    {
        for (int index = 0; index < 2; index++)
        {
            if (GrappleManager.Instance.grappleInteractions[index] != null)
            {
                GrappleManager.Instance.grappleInteractions[index].OnFixedUpdate();
                GrappleManager.Instance.grappleInteractions[index].OnSwing(swingVelocity[index]);
                if(reelingIn[index]){
                    GrappleManager.Instance.grappleInteractions[index].OnReelIn(reelInInput[index]);
                }
                if (reelingOut[index])
                {
                    GrappleManager.Instance.grappleInteractions[index].OnReelOut();
                }
            }
        }
    }

    public void SetReelingIn(int index, float reelInput){
        reelingIn[index] = reelInput > GrappleManager.Instance.options.reelDeadZone;
        reelInInput[index] = reelInput;
    }

    public void SetReelingOut(int index, bool isReelingOut)
    {
        reelingOut[index] = isReelingOut;
    }

    public void SetSwingVelocity(int index, Vector3 velocity){
        swingVelocity[index] = velocity;
    }
}
