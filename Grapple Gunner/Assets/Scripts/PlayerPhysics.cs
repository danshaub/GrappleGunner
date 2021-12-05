using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if(grounded && readyToJump) {
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
        if(PlayerManager._instance.allowMovement){
            Vector2 input = context.ReadValue<Vector2>();

            x = input.x;
            y = input.y;
        }
    }

    private void ContinuousMoveCancel(InputAction.CallbackContext context){
        x = 0;
        y = 0;
    }

    #endregion
}
