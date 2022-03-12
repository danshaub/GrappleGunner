using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueInteraction : I_GrappleInteraction
{
    public void OnHit(Transform gunTip, Transform hookPoint, GrapplePoint grapplePoint, int index){
        GrappleManager.Instance.guns[index].lightening.SetColor(GrappleManager.Instance.LighteningColors.blueColor);
    }
    public void OnRelease(){
        Debug.Log("Blue Release");
    }
    public void OnFixedUpdate(){
        Debug.Log("Blue F_Update");
    }
    public void OnReelIn(float reelStrength){
        Debug.Log("Blue R_In");
    }
    public void OnReelOut(){
        Debug.Log("Blue R_Out");
    }
    public void OnSwing(Vector3 swingVelocity){
        Debug.Log("Blue R_Swing");
    }
}
