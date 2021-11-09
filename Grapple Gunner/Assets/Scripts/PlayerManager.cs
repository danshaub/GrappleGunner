using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool allowGrapple = true;
    public bool allowMovement = true;
    public GrappleState grappleState = GrappleState.None;

    public enum GrappleState{
        Red,
        None
    }

}
