using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 50f;
    public float maxSpeed = 10f;
    public float jumpStrength = 4f;
    public LayerMask whatIsGround;
    public float airborneMoveStrength = .1f;
    public float coyoteTime = 3f;
    public float rayCastHeightOffset = 0.5f;
    public float colliderHeightOffset = .35f;
    public bool allowMovement = true;

    public bool isGrounded { get; private set; }
    public Vector2 moveInput { get; set; }
    public Vector3 horizontalVelocity { get; private set; }
    public bool JumpInput { get; set; }
    public bool jumped { get; private set; }
    public Vector3 groundNormal { get; private set; }

    [Header("Custom Physics")]
    public float gravityStrength = 9.8f;
    public bool useGravity = true;
    public float groundMaxNormal = 60f;
    public float frictionCoefficient = 1f;

    [SerializeField] private Transform headTransform;

    new private Rigidbody rigidbody;
    private CapsuleCollider playerCollider;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            JumpInput = true;
        }

    }

    private void FixedUpdate()
    {
        FollowPhysicalPlayer();
        horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        HandleGround();
        if(useGravity) ApplyGravity();
        ApplyFriction();

        if(allowMovement) Move();
    }

    // Resets player collider to reflect the current location of the headset.
    private void FollowPhysicalPlayer()
    {
        // Player Height
        float newHeight = (headTransform.localPosition.y + colliderHeightOffset) - rayCastHeightOffset;
        playerCollider.height = (newHeight >= playerCollider.radius * 2) ? newHeight : playerCollider.radius * 2;

        // Reset Collider Center
        playerCollider.center = new Vector3(headTransform.localPosition.x, (playerCollider.height / 2) + rayCastHeightOffset, headTransform.localPosition.z);


        // Send player height and offset info to PlayerManager
        PlayerManager._instance.playerHeight = headTransform.localPosition.y;
        PlayerManager._instance.playerXZLocalPosistion = new Vector3(headTransform.localPosition.x, 0, headTransform.localPosition.z);
    }

    private bool cancellingGrounded;
    private void HandleGround()
    {
        Vector3 rayCastOrigin = playerCollider.center;
        rayCastOrigin = transform.TransformPoint(rayCastOrigin);

        Vector3 targetPosition = transform.position;

        RaycastHit hit;

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, (playerCollider.height * .5f) + rayCastHeightOffset, whatIsGround) &&
           Mathf.Abs(Vector3.Angle(hit.normal, Vector3.up)) < groundMaxNormal){
            CancelInvoke("CoyoteTime");
            isGrounded = true;
            jumped = false;

            targetPosition.y = hit.point.y;
            groundNormal = hit.normal;
        }
        else{
            groundNormal = Vector3.zero;
            if(cancellingGrounded) Invoke("CoyoteTime", coyoteTime);
            cancellingGrounded = true;
        }

        if(isGrounded && !jumped){
            transform.position = targetPosition;
        }
    }

    private void CoyoteTime(){
        isGrounded = false;
        cancellingGrounded = false;
    }

    private void ApplyGravity(){
        if(!isGrounded){
            rigidbody.AddForce(Vector3.down * 9.8f, ForceMode.Acceleration);
        }
    }

    private void ApplyFriction(){
        if(isGrounded){
            rigidbody.AddForce(-rigidbody.velocity * frictionCoefficient, ForceMode.Force);
        }
    }
   

    private void Move()
    {
        // Movement
        if(moveInput.magnitude > float.Epsilon){
            Vector3 transformedInput = TransformInputToMoveDirection(moveInput);
            transformedInput = DampenMoveInput(transformedInput);
            Vector3 moveForce = transformedInput * acceleration;
            moveForce = isGrounded ? moveForce : moveForce * airborneMoveStrength;

            rigidbody.AddForce(moveForce, ForceMode.VelocityChange);
        }

        // Jump
        if (JumpInput && !jumped && isGrounded)
        {
            Debug.Log("Jump");
            rigidbody.MovePosition(transform.position + (transform.up * .01f));
            rigidbody.AddForce(transform.up * jumpStrength, ForceMode.VelocityChange);
            JumpInput = false;
            jumped = true;
            isGrounded = false;
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
    }

    // Limits the strength of movement inputs depending on the velocity of the player
    public Vector3 DampenMoveInput(Vector3 moveInput)
    {
        moveInput = moveInput - (Math.Min(horizontalVelocity.magnitude, maxSpeed) / maxSpeed) * horizontalVelocity.normalized;

        return moveInput;
    }
}