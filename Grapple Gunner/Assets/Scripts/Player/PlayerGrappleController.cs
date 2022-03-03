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
            if (GrappleManager._instance.grappleInteractions[index] != null)
            {
                GrappleManager._instance.grappleInteractions[index].OnFixedUpdate();
                GrappleManager._instance.grappleInteractions[index].OnSwing(swingVelocity[index]);
                if(reelingIn[index]){
                    GrappleManager._instance.grappleInteractions[index].OnReelIn(reelInInput[index]);
                }
                else if (reelingOut[index])
                {
                    GrappleManager._instance.grappleInteractions[index].OnReelOut();
                }
            }
        }
    }

    public void SetReelingIn(int index, float reelInput){
        reelingIn[index] = reelInput > GrappleManager._instance.options.reelDeadZone;
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
