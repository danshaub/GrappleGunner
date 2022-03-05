using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RedOptions : ScriptableObject
{
    public float redGrappleSpeed;
    public float redVelocityDamper;
    public AnimationCurve redVelocityCurve;
}
