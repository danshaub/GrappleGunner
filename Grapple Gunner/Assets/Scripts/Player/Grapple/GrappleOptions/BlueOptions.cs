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

    public PhysicMaterial heldPhysicsMaterial;
    public PhysicMaterial releasedPhysicsMaterial;
}
