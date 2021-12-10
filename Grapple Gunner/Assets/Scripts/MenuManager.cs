using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class MenuManager : MonoBehaviour
{
    public GameObject xrRig;
    private Slider contTurnSlider;
    private Slider snapTurnSlider;
    
    public void incrementSnapTurn()
    {
       // xrRig.turnSpeed += 5;
    }

    public void setContSlider(float val)
    {
        contTurnSlider.value = val;
    }
    public void setSnapSlider(float val)
    {
        snapTurnSlider.value = val;
    }
}
