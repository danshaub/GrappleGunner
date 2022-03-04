using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueInteraction : I_GrappleInteraction
{
    public void OnHit(Transform gunTip, Transform hookPoint){
        Debug.Log("Blue Hit");
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
