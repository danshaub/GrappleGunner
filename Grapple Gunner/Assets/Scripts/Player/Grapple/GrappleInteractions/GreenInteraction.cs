using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenInteraction : I_GrappleInteraction
{
    public void OnHit(){
        Debug.Log("GreenHit");
    }
    public void OnRelease(){
        Debug.Log("GreenRelease");
    }
    public void OnFixedUpdate(){
        Debug.Log("GreenF_Update");
    }
    public void OnReelIn(){
        Debug.Log("GreenR_In");
    }
    public void OnReelOut(){
        Debug.Log("GreenR_Out");
    }
    public void OnSwing(){
        Debug.Log("GreenR_Swing");
    }
}
