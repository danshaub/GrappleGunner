using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Reference: https://www.youtube.com/watch?v=XAC8U9-dTZU&ab_channel=DanisTutorials
public class PlayerManager : MonoBehaviour
{
    // Assigned Orientation
    public Transform orientation;

    // Player Rigidbody
    private Rigidbody rb;

    // Rotation and Look
    private float xRotation;
    
    // Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    // Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    // Collisions
    private Vector3 normalVector = Vector3.up;
    
    // Comfort
    public bool continuousMoveEnabled = true;

    //STAND IN VALUES FOR MOVEMENT CONTROL VECTOR
    float x = 0, y = 0;
    bool jumping;


    // Controller Input Actions
    public InputActionReference jumpReference = null;
    public InputActionReference moveReference = null;


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
        if(continuousMoveEnabled || !grounded){
            Movement();
        }
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        jumpReference.action.started -= JumpStart;
        jumpReference.action.canceled -= JumpCancel;
        moveReference.action.performed -= ContinuousMove;
        moveReference.action.canceled -= ContinuousMove;
    }

    private void Movement(){
    
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
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
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
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    private void Jump(){
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
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
    
    
    // Input action functions
    #region InputActions
    private void JumpStart(InputAction.CallbackContext context)
    {
        jumping = true;

        Debug.Log("Start Jump");
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        jumping = false;

        Debug.Log("Stop Jump");
    }

    private void ContinuousMove(InputAction.CallbackContext context){
        Vector2 input = context.ReadValue<Vector2>();

        x = input.x;
        y = input.y;

        Debug.Log("X: " + x.ToString() + "  Y: " + y.ToString());
    }

    private void ContinuousMoveCancel(InputAction.CallbackContext context){
        x = 0;
        y = 0;

        Debug.Log("X: " + x.ToString() + "  Y: " + y.ToString());
    }

    #endregion
}
