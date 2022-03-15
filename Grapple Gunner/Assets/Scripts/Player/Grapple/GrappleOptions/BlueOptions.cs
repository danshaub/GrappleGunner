using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueOptions", menuName = "Grapple Gunner/BlueOptions", order = 5)]
public class BlueOptions : ScriptableObject
{
    public float hookMass;
    public Vector3 targetHookPosition;
    public Vector3 targetHookRotation;

    public float springStrength;
    public float torsionalSpringStrength = 100;
    public float rotationalSlerpValue = 0.5f;
    public float launchForce;

    public float storeBlockInputThreshold = 0.25f;
    public float miniPointScale = 0.05f;
    public Vector3 miniPointLocalPosition;
    public float miniPointInterpolation = 0.1f;

    public PhysicMaterial heldPhysicsMaterial;
    public PhysicMaterial releasedPhysicsMaterial;
}
