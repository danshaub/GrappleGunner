using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    // Controller Prperties
    public float jumpAmount = 5;
    public float moveSpeed = 5;

    public bool continuousMoveEnabled = true;

    private Vector3 movementVector = Vector3.zero;
  

    // Controller Input Actions
    public InputActionReference jumpReference = null;
    public InputActionReference moveReference = null;

    private Rigidbody rb;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // subscribe to events
        jumpReference.action.started += Jump;
        moveReference.action.performed += ContinuousMove;
        
    }

    private void FixedUpdate() {
        #region Move Player

        rb.MovePosition(transform.position + Time.fixedDeltaTime * moveSpeed * transform.TransformDirection(movementVector));

        #endregion
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        jumpReference.action.started -= Jump;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        GetComponent<Rigidbody>().velocity = Vector3.up * jumpAmount;
    }

    private void ContinuousMove(InputAction.CallbackContext context){
        if(continuousMoveEnabled) {
            Vector2 controlValue = context.ReadValue<Vector2>();
            movementVector = new Vector3(controlValue.x, 0, controlValue.y);
        }
        else{
            movementVector = Vector3.zero;
        }
    }
}
