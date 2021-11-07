using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public bool allowGrapple = true;
    [HideInInspector] public bool allowMovement = true;
    [HideInInspector] public GrappleState grappleState = GrappleState.None;

    public enum GrappleState{
        Red,
        None
    }

}
