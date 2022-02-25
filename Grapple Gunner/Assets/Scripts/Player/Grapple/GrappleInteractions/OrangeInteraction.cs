using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeInteraction : I_GrappleInteraction
{
    public void OnHit(){
        Debug.Log("Orange Hit");
    }
    public void OnRelease(){
        Debug.Log("Orange Release");
    }
    public void OnFixedUpdate(){
        Debug.Log("Orange F_Update");
    }
    public void OnReelIn(){
        Debug.Log("Orange R_In");
    }
    public void OnReelOut(){
        Debug.Log("Orange R_Out");
    }
    public void OnSwing(){
        Debug.Log("Orange R_Swing");
    }
}
