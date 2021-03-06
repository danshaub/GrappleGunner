using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public PlayerMovementOptions options;
    public bool isGrounded { get; private set; }
    public Vector2 moveInput { private get; set; }
    public Vector3 horizontalVelocity { get; private set; }
    public bool JumpInput { private get; set; }
    public bool jumpCooldown { get; private set; }
    public Vector3 groundNormal { get; private set; }

    [SerializeField] private Transform headTransform;

    new public Rigidbody rigidbody { get; private set; }
    private Rigidbody groundBody;
    private Vector3 groundVelocity = Vector3.zero;
    public CapsuleCollider playerCollider { get; private set; }
    public List<Collider> gunColliders;
    private bool bufferingJumpInput;
    private float rideHeightDifference;

    private float footstepTimer = 0;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

    }
    private void FixedUpdate()
    {
        FollowPhysicalPlayer();
        horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        HandleGround();

        if (PlayerManager.Instance.useGravity) ApplyGravity();
        if (!GrappleManager.Instance.redConnected && PlayerManager.Instance.allowMovement) Move();

        if (JumpInput && !bufferingJumpInput)
        {
            StartCoroutine(JumpBuffer());
        }

        HandleFootsteps();
        HandleWoosh();
    }

    // Resets player collider to reflect the current location of the headset.
    private void FollowPhysicalPlayer()
    {
        // Player Height
        float newHeight = (headTransform.localPosition.y + options.colliderHeightOffset) - options.rideHeight;
        playerCollider.height = (newHeight >= playerCollider.radius * 2) ? newHeight : playerCollider.radius * 2;

        // Reset Collider Center
        playerCollider.center = new Vector3(headTransform.localPosition.x, (playerCollider.height / 2) + options.rideHeight, headTransform.localPosition.z);


        // Send player height and offset info to PlayerManager
        PlayerManager.Instance.playerHeight = headTransform.localPosition.y;
        PlayerManager.Instance.playerXZLocalPosistion = new Vector3(headTransform.localPosition.x, 0, headTransform.localPosition.z);
    }

    private bool wasGrounded = true;
    private void HandleGround()
    {
        Vector3 rayCastOrigin = transform.TransformPoint(playerCollider.center);
        float targetCenterHeight = (playerCollider.height * .5f) + options.rideHeight;
        float rayCastLength = (playerCollider.height * .5f) + (options.groundSnapDistance);

        RaycastHit hit;

        Debug.DrawLine(rayCastOrigin, rayCastOrigin - (Vector3.up * rayCastLength), Color.blue);
        Debug.DrawLine(rayCastOrigin, rayCastOrigin - (Vector3.up * targetCenterHeight), Color.black);

        if (Physics.Raycast(rayCastOrigin, Vector3.down, out hit, rayCastLength, options.whatIsGround) &&
           Mathf.Abs(Vector3.Angle(hit.normal, Vector3.up)) < options.groundMaxNormal)
        {
            groundNormal = hit.normal;


            groundBody = hit.rigidbody;
            if (groundBody != null)
            {
                groundVelocity = groundBody.velocity;
            }
            else
            {
                groundVelocity = Vector3.zero;
            }

            float rayCastDirectionalVelocity = Vector3.Dot(Vector3.down, rigidbody.velocity);
            float otherDirectionalVelocity = Vector3.Dot(Vector3.down, groundVelocity);

            float relativeVelocity = rayCastDirectionalVelocity - otherDirectionalVelocity;

            rideHeightDifference = hit.distance - targetCenterHeight;
            isGrounded = rideHeightDifference <= options.groundSnapDistance * .5f;

            if (!wasGrounded && isGrounded)
            {
                SFXManager.Instance.PlaySFX("PlayerLanding");
            }
            wasGrounded = isGrounded;

            float springForce = (rideHeightDifference * options.rideSpringStrength) - (relativeVelocity * options.rideSpringDamper);

            if (GrappleManager.Instance.groundCheck)
            {
                if (!jumpCooldown)
                {
                    if (GrappleManager.Instance.redConnected)
                    {
                        springForce = Mathf.Clamp(springForce, float.MinValue, 0f);
                    }
                    rigidbody.AddForce(Vector3.down * springForce);
                }

                if (groundBody != null)
                {
                    groundBody.AddForceAtPosition(Vector3.down * -(springForce) * .1f, hit.point);
                    groundBody.AddForceAtPosition(Vector3.down * rigidbody.mass * options.massMultiplier, hit.point);
                }

                if (!GrappleManager.Instance.redConnected && isGrounded)
                {
                    rigidbody.AddForce((groundVelocity - horizontalVelocity) * options.frictionCoefficient, ForceMode.Force);
                }
            }

            DisablePlayerMovement dpmGround = hit.transform.gameObject.GetComponent<DisablePlayerMovement>();
            if (dpmGround != null)
            {
                PlayerManager.Instance.allowMovement = !dpmGround.active;
            }
            else
            {
                PlayerManager.Instance.allowMovement = true;
            }

            DisableGrapple dgGround = hit.transform.gameObject.GetComponent<DisableGrapple>();
            if (dgGround != null)
            {
                GrappleManager.Instance.SetGrappleDisabled(dgGround.active);
            }
            else
            {
                GrappleManager.Instance.SetGrappleDisabled(false);
            }

            DeathBlock deathBlockGround = hit.transform.gameObject.GetComponent<DeathBlock>();
            if (deathBlockGround != null)
            {
                deathBlockGround.KillPlayer();
            }

            LevelComplete loadLevel = hit.transform.gameObject.GetComponent<LevelComplete>();
            if (loadLevel != null)
            {
                loadLevel.Activate();
            }

            DisableHandleldMenu disableMenu = hit.transform.gameObject.GetComponent<DisableHandleldMenu>();
            if (disableMenu != null)
            {
                MenuManager.Instance.menuLocked = true;
            }
            else
            {
                MenuManager.Instance.menuLocked = false;
            }
        }
        else
        {
            groundNormal = Vector3.zero;
            isGrounded = false;
            transform.parent = null;
            groundVelocity = Vector3.zero;
        }

        if (PlayerManager.Instance.useGrapplePhysicsMaterial)
        {
            playerCollider.material = options.grappleMaterial;
            foreach (Collider c in gunColliders)
            {
                c.material = options.grappleMaterial;
            }
        }
        else if (isGrounded)
        {
            playerCollider.material = options.groundedMaterial;
            foreach (Collider c in gunColliders)
            {
                c.material = options.groundedMaterial;
            }
        }
        else
        {
            playerCollider.material = options.airborneMaterial;
            foreach (Collider c in gunColliders)
            {
                c.material = options.airborneMaterial;
            }
        }

        if (transform.parent?.GetComponent<Rigidbody>() != null)
        {
        }

    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rigidbody.AddForce(Vector3.down * options.gravityStrength, ForceMode.Acceleration);
        }
    }

    private void Move()
    {
        // Movement
        if (moveInput.magnitude > float.Epsilon)
        {
            Vector3 transformedInput = TransformInputToMoveDirection(moveInput);
            transformedInput = DampenMoveInput(transformedInput);
            Vector3 moveForce = transformedInput * options.acceleration;
            moveForce = isGrounded ? moveForce : moveForce * options.airborneMoveStrength;

            rigidbody.AddForce(moveForce, ForceMode.VelocityChange);
        }

        // Jump
        if (JumpInput && !jumpCooldown && isGrounded)
        {

            JumpInput = false;
            StartCoroutine(Jump());
        }
    }

    private IEnumerator Jump()
    {

        while (rideHeightDifference > options.maxJumpRideHeightDifference)
        {
            yield return new WaitForFixedUpdate();
        }

        jumpCooldown = true;
        yield return new WaitForFixedUpdate();
        rigidbody.AddForce(Vector3.up * options.jumpStrength);
        SFXManager.Instance.PlaySFX("PlayerJump");

        yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(options.jumpCooldown);

        jumpCooldown = false;
    }

    private IEnumerator JumpBuffer()
    {
        bufferingJumpInput = true;
        for (int i = 0; i < options.jumpBufferFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        bufferingJumpInput = false;
        JumpInput = false;
    }

    // public void PauseGroundSnap()
    // {
    //     CancelInvoke("PauseTimer");
    //     jumpCooldown = true;
    //     Invoke("PauseTimer", options.jumpCooldown);
    // }
    // // Signals enough time has passed since jumping resume snapping to ground
    // void PauseTimer()
    // {
    //     jumpCooldown = false;
    // }

    // Ensures movement is applied in the correct direction relative to the direction the HMD is pointing
    public Vector3 TransformInputToMoveDirection(Vector2 inputVector)
    {
        float lookAngle = -headTransform.transform.eulerAngles.y;

        float x = (float)(inputVector.x * Mathf.Cos(Mathf.Deg2Rad * lookAngle) - inputVector.y * Mathf.Sin(Mathf.Deg2Rad * lookAngle));
        float y = (float)(inputVector.x * Mathf.Sin(Mathf.Deg2Rad * lookAngle) + inputVector.y * Mathf.Cos(Mathf.Deg2Rad * lookAngle));


        Vector3 transformedInput = new Vector3(x, 0, y);

        return transformedInput;
    }

    // Limits the strength of movement inputs depending on the velocity of the player
    public Vector3 DampenMoveInput(Vector3 moveInput)
    {
        moveInput = moveInput - (Math.Min(horizontalVelocity.magnitude, options.maxSpeed) / options.maxSpeed) * horizontalVelocity.normalized;

        return moveInput;
    }

    private void HandleFootsteps()
    {
        if (!isGrounded)
        {
            footstepTimer = options.groundedStepTime;
            return;
        }
        if (moveInput == Vector2.zero)
        {
            footstepTimer = options.initialStepTime;
            return;
        }

        footstepTimer -= Time.fixedDeltaTime;

        if (footstepTimer <= 0)
        {
            SFXManager.Instance.PlaySFX("PlayerWalking");
            footstepTimer = Mathf.Clamp(options.stepSpeedMultiplier / horizontalVelocity.magnitude, options.minStepTime, options.maxStepTime);
        }
    }

    private void HandleWoosh()
    {
        float targetVolume;
        float targetPitch;
        if (isGrounded)
        {
            targetVolume = options.wooshVolume.Evaluate(0);
            targetPitch = options.wooshPitch.Evaluate(0);
        }
        else
        {
            targetVolume = options.wooshVolume.Evaluate(rigidbody.velocity.magnitude);
            targetPitch = options.wooshPitch.Evaluate(rigidbody.velocity.magnitude);
        }

        SFXManager.Instance.SetSFXVolume("Woosh", Mathf.Lerp(SFXManager.Instance.GetSFXVolume("Woosh"), targetVolume, options.wooshDecay), true);
        SFXManager.Instance.SetSFXPitch("Woosh", Mathf.Lerp(SFXManager.Instance.GetSFXPitch("Woosh"), targetPitch, options.wooshDecay));
    }

    private bool bonked = false;
    private void OnCollisionEnter(Collision other)
    {
        if (isGrounded) return;

        if (bonked) return;

        float hitVelocity = Vector3.Dot(other.relativeVelocity, other.contacts[0].normal);
        if (hitVelocity > options.wallBonkThreshold)
        {
            bonked = true;
            SFXManager.Instance.SetSFXVolume("PlayerWallBonk", options.wallBonkVolume.Evaluate(hitVelocity), true);
            StartCoroutine(WallBonkCooldown());
        }
    }

    private IEnumerator WallBonkCooldown()
    {
        yield return new WaitForSeconds(options.wallBonkCooldown);
        bonked = false;
    }
}