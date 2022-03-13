using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GreenOptions", menuName = "Grapple Gunner/GreenOptions", order = 3)]
public class GreenOptions : ScriptableObject
{
    public float limitContactDistance;
    public float swingDamper;
    public float swingVelocityThreshold;
    public float maxSwingVelocity;
    public float swingForceMultiplier;
    public float doubleGreenMultiplier;
    public float snapDistanceMultiplier;
    public float greenReelForce;
    public float groundedReelMultiplier;
    public float greenSnapSpeed;
    public float greenSnapVelocityDamper;
    public AnimationCurve greenSnapVelocityCurve;
    public float greenMaxSlackSpeed;
    public float reelDeadZone;
    public float greenSlackDamper;
}
