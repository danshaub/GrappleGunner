using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerController playerController;

    // Controller Input Actions
    public InputActionReference jumpReference = null;
    public InputActionReference moveReference = null;

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        jumpReference.action.started += JumpStart;
        jumpReference.action.canceled += JumpCancel;
        moveReference.action.performed += ContinuousMove;
        moveReference.action.canceled += ContinuousMove;
    }
    private void OnDestroy()
    {
        jumpReference.action.started -= JumpStart;
        jumpReference.action.canceled -= JumpCancel;
        moveReference.action.performed -= ContinuousMove;
        moveReference.action.canceled -= ContinuousMove;
    }

    // Input action functions
    #region InputActions
    private void JumpStart(InputAction.CallbackContext context)
    {
        playerController.JumpInput = true;
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        playerController.JumpInput = false;
    }

    private void ContinuousMove(InputAction.CallbackContext context)
    {
        Vector2 input;
        if (PlayerManager._instance.allowMovement)
        {
            input = context.ReadValue<Vector2>();

        }
        else
        {
            input = Vector2.zero;
        }

        playerController.LateralMovement = input;
    }
    #endregion
}
