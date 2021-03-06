using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementOptions", menuName = "Grapple Gunner/PlayerMovementOptions", order = 0)]
public class PlayerMovementOptions : ScriptableObject {
    [Header("General Options")]
    public float acceleration = 5f;
    public float maxSpeed = 15f;
    public float jumpStrength = 400f;
    public float jumpCooldown = 0.1f;
    public LayerMask whatIsGround;
    public float airborneMoveStrength = 0.05f;
    public float maxJumpRideHeightDifference = 0.25f;
    public float rideHeight = 0.25f;
    public float rideSpringStrength = 1;
    public float rideSpringDamper = 1;
    public float groundSnapDistance = 0.3f;
    public float colliderHeightOffset = .35f;
    public int jumpBufferFrames = 5;

    [Header("Custom Physics")]
    public float gravityStrength = 9.8f;
    public float groundMaxNormal = 60f;
    public float frictionCoefficient = 15f;
    public float massMultiplier = 10;

    public PhysicMaterial groundedMaterial;
    public PhysicMaterial airborneMaterial;
    public PhysicMaterial grappleMaterial;

    [Header("SFX Parameters")]
    public float stepSpeedMultiplier = 1f;
    public float maxStepTime = 1f;
    public float minStepTime = 0.01f;
    public float groundedStepTime = .35f;
    public float initialStepTime = .1f;

    public float wallBonkThreshold = 5f;
    public float wallBonkCooldown = 0.25f;
    public AnimationCurve wallBonkVolume;

    public AnimationCurve wooshVolume;
    public AnimationCurve wooshPitch;
    public float wooshDecay = .1f;

}
