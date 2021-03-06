using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueOptions", menuName = "Grapple Gunner/BlueOptions", order = 5)]
public class BlueOptions : ScriptableObject
{
    public float hookMass;
    public Vector3 targetHookPosition;
    public Vector3 storingTargetHookPosition;

    public float springStrength;
    public float maxHookVelocity = 500;
    public float maxSpringForce = 1500f;
    public float velocityClampDistance = 4f;
    public float launchForce;

    public float storeBlockInputThreshold = 0.25f;
    public float miniPointScale = 0.05f;
    public Vector3 miniPointLocalPosition;
    public float interpolationValue = 0.1f;
    public float storeBlockCooldown = 0.5f;
    public float maxPushbackForce = 2000f;

    public float destructionTime = 0.25f;
    public GameObject destructionPS;
    public GameObject respawnPS;

    public PhysicMaterial heldPhysicsMaterial;
    public PhysicMaterial releasedPhysicsMaterial;

    public LayerMask invalidTakeOutLayers;

    [Header("SFX")]
    public float bonkCooldown;
    public float bonkThreshold;
    public AnimationCurve bonkVolume;
    public AudioClip bonkSound;
    public AudioClip bonkBarrierSound;

    public float spawnVolume;
    public AudioClip destroySound;
    public AudioClip respawnSound;
}
