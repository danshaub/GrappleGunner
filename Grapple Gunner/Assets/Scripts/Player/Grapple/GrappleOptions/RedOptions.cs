using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RedOptions", menuName = "Grapple Gunner/RedOptions", order = 2)]
public class RedOptions : ScriptableObject
{
    public float redGrappleSpeed;
    public float redVelocityDamper;
    public AnimationCurve redVelocityCurve;
}
