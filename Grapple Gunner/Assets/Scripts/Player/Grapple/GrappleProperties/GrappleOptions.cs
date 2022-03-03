using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GrappleOptions : ScriptableObject
{
    [Header("Hook Options")]
    public float hookTravelSpeed;
    public float retractInterpolateValue;
    public float sphereCastRadius;
    public LayerMask sphereCastMask;

    [Header("Reticle Options")]
    public ReticleTextures reticleManager;
    public float disabledTransparency;
    public float minReticleDistance;
    public float maxReticleDistance;
    public AnimationCurve reticleScaleCurve;
}
