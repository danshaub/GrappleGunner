using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedInteraction : I_GrappleInteraction
{
    public void OnHit(){
        Debug.Log("Red Hit");
    }
    public void OnRelease(){
        Debug.Log("Red Release");
    }
    public void OnFixedUpdate(){
        Debug.Log("Red F_Update");
    }
    public void OnReelIn(){
        Debug.Log("Red R_In");
    }
    public void OnReelOut(){
        Debug.Log("Red R_Out");
    }
    public void OnSwing(){
        Debug.Log("Red R_Swing");
    }
}
