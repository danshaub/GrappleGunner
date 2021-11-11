using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // public GrappleGun leftGun;
    public GrappleGun rightGun;
    public bool allowGrapple = true;
    public bool allowMovement = true;

    public bool grounded = true;
    public GrappleState grappleState = GrappleState.None;

    public enum GrappleState{
        Red,
        None
    }

}
