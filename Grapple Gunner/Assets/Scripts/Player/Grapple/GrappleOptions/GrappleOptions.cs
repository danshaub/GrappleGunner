using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrappleOptions", menuName = "Grapple Gunner/GrappleOptions", order = 1)]
public class GrappleOptions : ScriptableObject
{
    [Header("Gun Options")]
    public float swingVelocityThreshold;
    public float reelDeadZone;
    [Header("Hook Options")]
    public float hookTravelSpeed;
    public float retractInterpolateValue;
    public float returnSnapDistance;
    public float sphereCastRadius;
    public float timeBeforeRetract;
    public LayerMask sphereCastMask;

    [Header("Reticle Options")]
    public ReticleTextures reticleManager;
    public float disabledTransparency;
    public float minReticleDistance;
    public float maxReticleDistance;
    public AnimationCurve reticleScaleCurve;
}