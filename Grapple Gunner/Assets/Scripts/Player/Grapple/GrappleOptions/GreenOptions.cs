using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GreenOptions", menuName = "Grapple Gunner/GreenOptions", order = 3)]
public class GreenOptions : ScriptableObject
{
    [Header("General")]
    public float springForce = 1000;
    [Range(0,1)]
    public float springDamper = 0.1f;
    public float forceGroundedMultiplier = 3;
    public AnimationCurve forceDistanceMultiplier;
    public float minLinearLimit = 0.1f;
    public float contactVelocityDamper = 0.1f;

    [Header("Swing Options")]
    public float swingInputThreshold = .25f;
    public float maxSwingVelocity = 3.5f;
    public float swingForce = 150f;

    [Range(0,1)]
    public float doubleGreenMultiplier = 0.75f;

    [Header("Reel In Options")]
    public float reelInForce = 100f;
    public AnimationCurve reelInDistanceVelocityDamper;
    public float reelInDeadZone = 0.1f;

    [Header("Reel Out Options")]
    public float maxReelOutSpeed = 5f;
    [Range(0,1)]
    public float reelOutDamper = 0.1f;

}
