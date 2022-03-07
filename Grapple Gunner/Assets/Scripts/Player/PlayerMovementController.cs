using System;
using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public PlayerMovementOptions options;
    public bool isGrounded { get; private set; }
    public Vector2 moveInput { private get; set; }
    public Vector3 horizontalVelocity { get; private set; }
    public bool JumpInput { private get; set; }
    public bool pauseGroundSnap { get; private set; }
    public Vector3 groundNormal { get; private set; }

    [SerializeField] private Transform headTransform;
    [SerializeField] private PhysicMaterial groundedMaterial;
    [SerializeField] private PhysicMaterial airborneMaterial;
    [SerializeField] private PhysicMaterial grappleMaterial;

    new public Rigidbody rigidbody { get; private set; }
    private CapsuleCollider playerCollider;

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
        ApplyFriction();

        if (PlayerManager.Instance.allowMovement) Move();
    }

    // Resets player collider to reflect the current location of the headset.
    private void FollowPhysicalPlayer()
    {
        // Player Height
        float newHeight = (headTransform.localPosition.y + options.colliderHeightOffset) - options.rayCastHeightOffset;
        playerCollider.height = (newHeight >= playerCollider.radius * 2) ? newHeight : playerCollider.radius * 2;

        // Reset Collider Center
        playerCollider.center = new Vector3(headTransform.localPosition.x, (playerCollider.height / 2) + options.rayCastHeightOffset, headTransform.localPosition.z);


        // Send player height and offset info to PlayerManager
        PlayerManager.Instance.playerHeight = headTransform.localPosition.y;
        PlayerManager.Instance.playerXZLocalPosistion = new Vector3(headTransform.localPosition.x, 0, headTransform.localPosition.z);
    }

    private void HandleGround()
    {
        Vector3 rayCastOrigin = playerCollider.center;
        rayCastOrigin = transform.TransformPoint(rayCastOrigin);

        Vector3 targetPosition = transform.position;

        RaycastHit hit;

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, (playerCollider.height * .5f) + options.rayCastHeightOffset, options.whatIsGround) &&
           Mathf.Abs(Vector3.Angle(hit.normal, Vector3.up)) < options.groundMaxNormal)
        {
            isGrounded = true;

            targetPosition.y = hit.point.y;
            groundNormal = hit.normal;
        }
        else
        {
            groundNormal = Vector3.zero;
            isGrounded = false;
        }

        if (isGrounded && !pauseGroundSnap)
        {
            if (transform.position.y < targetPosition.y)
            {
                rigidbody.MovePosition(Vector3.Lerp(transform.position, targetPosition, options.landingBounce));
            }
            else
            {
                rigidbody.MovePosition(targetPosition);
            }
        }

        if(PlayerManager.Instance.useGrapplePhysicsMaterial){
            playerCollider.material = grappleMaterial;
        }
        else if(isGrounded){
            playerCollider.material = groundedMaterial;
        }
        else{
            playerCollider.material = airborneMaterial;
        }
        
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rigidbody.AddForce(Vector3.down * options.gravityStrength, ForceMode.Acceleration);
        }
    }

    private void ApplyFriction()
    {
        if (isGrounded)
        {
            rigidbody.AddForce(-rigidbody.velocity * options.frictionCoefficient, ForceMode.Force);
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
        if (JumpInput && !pauseGroundSnap && isGrounded)
        {
            rigidbody.velocity = horizontalVelocity;
            rigidbody.AddForce(transform.up * options.jumpStrength, ForceMode.Force);
            JumpInput = false;

            PauseGroundSnap();
        }
    }

    public void PauseGroundSnap()
    {
        CancelInvoke("PauseTimer");
        pauseGroundSnap = true;
        Invoke("PauseTimer", options.pauseTimer);
    }
    // Signals enough time has passed since jumping resume snapping to ground
    void PauseTimer()
    {
        pauseGroundSnap = false;
    }

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
}