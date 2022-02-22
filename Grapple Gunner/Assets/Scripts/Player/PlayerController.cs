using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Physics Tuning")]
    [Tooltip("Maximum slope the character can jump on")]
    [Range(5f, 60f)]
    public float slopeLimit = 45f;
    [Tooltip("Max height at which character steps up automatically")]
    public LayerMask whatIsGround;
    public float stepHeight = 0.3f;
    public float stepSmooth = 0.1f;
    public float stepCheckDistance = 0.1f;
    [Tooltip("Movement acceleration")]
    public float acceleration = 50f;
    [Tooltip("Maximum lateral move speed in meters/second")]
    public float maxSpeed = 10f;
    [Tooltip("Determines strength of movement while airborne")]
    [Range(0f, 1f)]
    public float airborneMoveStrength = .1f;
    [Tooltip("Whether the character can jump")]
    public bool allowJump = true;
    [Tooltip("Upward force to apply when jumping in newtons")]
    public float jumpSpeed = 4f;
    public float coyoteTime = 3f;
    [Header("Physics Materials")]
    public PhysicMaterial groundedPhysicsMat;
    public PhysicMaterial airbornePhysicsMat;
    [Header("IK Configuration")]
    [Tooltip("Specifies distance between camera and top of collider")]
    public float colliderHeightOffset = .35f;

    public bool IsGrounded { get; private set; }
    public Vector2 LateralInput { get; set; }
    public Vector3 HorizontalVelocity { get; private set; }
    public bool JumpInput { get; set; }
    public bool AllowMovement
    {
        get; set;
    }

    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform stepRayLower;
    [SerializeField] private Transform stepRayUpper;

    new private Rigidbody rigidbody;
    private CapsuleCollider playerCollider;
    private bool steppedThisFrame;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        // stepRayUpper.position = new Vector3(stepRayUpper)
    }

    private void FixedUpdate()
    {
        steppedThisFrame = false;
        FollowPhysicalPlayer();
        HorizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        if(HorizontalVelocity.magnitude > 0.1f){
            StepClimb(transform.InverseTransformDirection(HorizontalVelocity.normalized));
        }

        Move();
    }

    // Resets player collider to reflect the current location of the headset.
    private void FollowPhysicalPlayer()
    {
        // Player Height
        float newHeight = headTransform.localPosition.y + colliderHeightOffset;
        playerCollider.height = (newHeight >= playerCollider.radius * 2) ? newHeight : playerCollider.radius * 2;

        // Reset Collider Center
        playerCollider.center = new Vector3(headTransform.localPosition.x, playerCollider.height / 2, headTransform.localPosition.z);


        // Send player height and offset info to PlayerManager
        PlayerManager._instance.playerHeight = headTransform.localPosition.y;
        PlayerManager._instance.playerXZLocalPosistion = new Vector3(headTransform.localPosition.x, 0, headTransform.localPosition.z);
    }
    
    #region Floor/Slope Collision
    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < slopeLimit;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                IsGrounded = true;
                cancellingGrounded = false;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * coyoteTime);
        }
    }

    public void StopGrounded()
    {
        IsGrounded = false;
    }

    // Checks if there is a stair step in front of the player and steps accordingly
    private void StepClimb(Vector3 checkDirection)
    {
        if(steppedThisFrame) return;

        stepRayUpper.localPosition = playerCollider.center + (Vector3.down * playerCollider.height * .5f) + (Vector3.up * stepHeight) ;
        stepRayLower.localPosition = playerCollider.center + (Vector3.down * playerCollider.height * .5f) + (Vector3.up * .05f);

        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.position, checkDirection, out hitLower, stepCheckDistance))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.position, checkDirection, out hitUpper, (stepCheckDistance + .1f)))
            {
                rigidbody.MovePosition(transform.position + (Vector3.up * stepSmooth));
                steppedThisFrame = true;
                return;
            }
        }

        checkDirection = Quaternion.AngleAxis(-45, Vector3.up) * checkDirection;
        Debug.Log(checkDirection);


        RaycastHit hitLowerLeft;
        if (Physics.Raycast(stepRayLower.position, checkDirection, out hitLowerLeft, stepCheckDistance))
        {
            RaycastHit hitUpperLeft;
            if (!Physics.Raycast(stepRayUpper.position, checkDirection, out hitUpperLeft, (stepCheckDistance + .1f)))
            {
                rigidbody.MovePosition(transform.position + (Vector3.up * stepSmooth));
                steppedThisFrame = true;
                return;
            }
        }

        checkDirection = Quaternion.AngleAxis(90, Vector3.up) * checkDirection;
        Debug.Log(checkDirection);


        RaycastHit hitLowerRight;
        if (Physics.Raycast(stepRayLower.position, checkDirection, out hitLowerRight, stepCheckDistance))
        {
            RaycastHit hitUpperRight;
            if (!Physics.Raycast(stepRayUpper.position, checkDirection, out hitUpperRight, (stepCheckDistance + .1f)))
            {
                rigidbody.MovePosition(transform.position + (Vector3.up * stepSmooth));
                steppedThisFrame = true;
                return;
            }
        }
    }

    /// <summary>
    /// Processes input actions and converts them into movement
    /// </summary>

    #endregion

    private void Move()
    {
        if(LateralInput.magnitude > float.Epsilon){
            Vector3 transformedInput = TransformInputToMoveDirection(LateralInput);
            transformedInput = DampenMoveInput(transformedInput);
            // Movement
            Vector3 moveForce = transformedInput * acceleration;
            moveForce = IsGrounded ? moveForce : moveForce * airborneMoveStrength;

            StepClimb(transformedInput.normalized);
            rigidbody.AddForce(moveForce, ForceMode.VelocityChange);
        }

        // Jump
        if (JumpInput && allowJump && IsGrounded)
        {
            rigidbody.AddForce(transform.up * jumpSpeed, ForceMode.VelocityChange);
            JumpInput = false;
        }
    }

    // Ensures movement is applied in the correct direction relative to the direction the HMD is pointing
    public Vector3 TransformInputToMoveDirection(Vector2 inputVector)
    {
        float lookAngle = -headTransform.transform.eulerAngles.y;

        float x = (float)(inputVector.x * Mathf.Cos(Mathf.Deg2Rad * lookAngle) - inputVector.y * Mathf.Sin(Mathf.Deg2Rad * lookAngle));
        float y = (float)(inputVector.x * Mathf.Sin(Mathf.Deg2Rad * lookAngle) + inputVector.y * Mathf.Cos(Mathf.Deg2Rad * lookAngle));


        Vector3 transformedInput = new Vector3(x, 0, y);

        return transformedInput;

        // return Quaternion.AngleAxis(90, Vector3.up) * (new Vector3(inputVector.x, 0, inputVector.y));
    }

    // Limits the strength of movement inputs depending on the velocity of the player
    public Vector3 DampenMoveInput(Vector3 moveInput)
    {
        moveInput = moveInput - (Math.Min(HorizontalVelocity.magnitude, maxSpeed) / maxSpeed) * HorizontalVelocity.normalized;

        return moveInput;
    }
}

/*
// Reference: https://www.youtube.com/watch?v=XAC8U9-dTZU&ab_channel=DanisTutorials
public class PlayerPhysics : MonoBehaviour
{
    public PlayerManager playerManager;
    // Assigned Orientation
    public Transform orientation;
    public Transform playerController;
    public CapsuleCollider playerCollider;
    public float colliderHeightOffset = .35f;

    // public TrackedDevice

    private float newHeight;

    // Player Rigidbody
    private Rigidbody rb;

    // Rotation and Look
    private float xRotation;
    
    // Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public float coyoteTime = 3f;
    public bool grounded;
    public bool touchingSurface;
    public LayerMask whatIsGround;
    public LayerMask whatDisablesPlayer;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    public float airDampeningMultiplier = 0.25f;

    // Projected look direction
    Vector3 forward;
    Vector3 right;

    // Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    // Collisions
    private Vector3 normalVector = Vector3.up;

    //STAND IN VALUES FOR MOVEMENT CONTROL VECTOR
    float x = 0, y = 0;
    bool jumping;


    // Controller Input Actions
    public InputActionReference jumpReference = null;
    public InputActionReference moveReference = null;

    public void ResetVelocity(){
        rb.velocity = Vector3.zero;
    }


    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // subscribe to events
        jumpReference.action.started += JumpStart;
        jumpReference.action.canceled += JumpCancel;
        moveReference.action.performed += ContinuousMove;
        moveReference.action.canceled += ContinuousMove;   
    }

    private void FixedUpdate() {
        FollowPhysicalPlayer();
        if(PlayerManager._instance.allowMovement || !grounded){
            Movement();
        }
        PlayerManager._instance.grounded = grounded;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        jumpReference.action.started -= JumpStart;
        jumpReference.action.canceled -= JumpCancel;
        moveReference.action.performed -= ContinuousMove;
        moveReference.action.canceled -= ContinuousMove;
    }

    // Resets player collider to reflect the current location of the headset.
    private void FollowPhysicalPlayer(){
        // Player Height
        newHeight = orientation.localPosition.y + colliderHeightOffset;
        playerCollider.height = (newHeight >= playerCollider.radius * 2) ? newHeight : playerCollider.radius * 2;

        // Reset Collider Center
        playerCollider.center = new Vector3(orientation.localPosition.x, playerCollider.height / 2, orientation.localPosition.z);


        // Send player height and offset info to PlayerManager
        PlayerManager._instance.playerHeight = orientation.localPosition.y;
        PlayerManager._instance.playerXZLocalPosistion = new Vector3(orientation.localPosition.x, 0, orientation.localPosition.z);
    }

    private void Movement(){

        forward = new Vector3(orientation.transform.forward.x, 0, orientation.transform.forward.z).normalized;
        right = new Vector3(orientation.transform.right.x, 0, orientation.transform.right.z).normalized;
        
        NonGrappleMovement();
    }

    private void NonGrappleMovement(){
        rb.useGravity = true;
        // Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        // Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        // Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        // If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        float maxSpeed = this.maxSpeed;

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = airDampeningMultiplier;
            multiplierV = airDampeningMultiplier;
        }

        //Apply forces to move player
        rb.AddForce(forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    public Vector2 FindVelRelativeToLook(){
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private void CounterMovement(float x, float y, Vector2 mag){
        if (!grounded || jumping) return;

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    public void Jump(){
        if(grounded && readyToJump && PlayerManager._instance.allowMovement) {
            readyToJump = false;

            // Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * .5f);

            // If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f){
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            }
            else if (rb.velocity.y > 0){
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            }

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump(){
        readyToJump = true;
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * coyoteTime);
        }
    }

    public void StopGrounded()
    {
        grounded = false;
    }
    
    
    // Input action functions
    #region InputActions
    private void JumpStart(InputAction.CallbackContext context)
    {
        jumping = true;
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        jumping = false;
    }

    private void ContinuousMove(InputAction.CallbackContext context){
        Vector2 input;
        if(PlayerManager._instance.allowMovement){
            input = context.ReadValue<Vector2>();

        }
        else{
            input = Vector2.zero;
        }

        x = input.x;
        y = input.y;
    }

    private void ContinuousMoveCancel(InputAction.CallbackContext context){
        x = 0;
        y = 0;
    }

    #endregion
}
*/