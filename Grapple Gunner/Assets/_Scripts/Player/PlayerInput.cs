using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : Singleton<PlayerInput>
{
    // Basic Input Actions
    public InputActionReference jumpReference = null;
    public InputActionReference moveReference = null;

    // Grapple Actions (Left)
    public InputActionReference fireReferenceLeft = null;
    public InputActionReference reelInReferenceLeft = null;
    public InputActionReference reelOutReferenceLeft = null;
    public InputActionReference swingReferenceLeft = null;
    // Grapple Actions (Right)
    public InputActionReference fireReferenceRight = null;
    public InputActionReference reelInReferenceRight = null;
    public InputActionReference reelOutReferenceRight = null;
    public InputActionReference swingReferenceRight = null;

    // Menu Actions
    public InputActionReference menuReference = null;

    // REMOVE WITH BUILDS
    public InputActionReference debugReference = null;

    // Start is called before the first frame update
    protected override void Awake()
    {
        jumpReference.action.started += JumpStart;
        jumpReference.action.canceled += JumpCancel;
        moveReference.action.performed += ContinuousMove;
        moveReference.action.canceled += ContinuousMove;

        menuReference.action.started += ShowMenu;
        menuReference.action.canceled += HideMenu;

        fireReferenceLeft.action.started += FireLeftHook;
        fireReferenceLeft.action.canceled += ReleaseLeftHook;
        reelOutReferenceLeft.action.started += ReelOutStartLeftHook;
        reelOutReferenceLeft.action.canceled += ReelOutEndLeftHook;
        fireReferenceRight.action.started += FireRightHook;
        fireReferenceRight.action.canceled += ReleaseRightHook;
        reelOutReferenceRight.action.started += ReelOutStartRightHook;
        reelOutReferenceRight.action.canceled += ReelOutEndRightHook;

        debugReference.action.performed += Debug;
    }
    private void OnDestroy()
    {
        jumpReference.action.started -= JumpStart;
        jumpReference.action.canceled -= JumpCancel;
        moveReference.action.performed -= ContinuousMove;
        moveReference.action.canceled -= ContinuousMove;

        menuReference.action.started -= ShowMenu;
        menuReference.action.canceled -= HideMenu;

        fireReferenceLeft.action.started -= FireLeftHook;
        fireReferenceLeft.action.canceled -= ReleaseLeftHook;
        reelOutReferenceLeft.action.started -= ReelOutStartLeftHook;
        reelOutReferenceLeft.action.canceled -= ReelOutEndLeftHook;
        fireReferenceRight.action.started -= FireRightHook;
        fireReferenceRight.action.canceled -= ReleaseRightHook;
        reelOutReferenceRight.action.started -= ReelOutStartRightHook;
        reelOutReferenceRight.action.canceled -= ReelOutEndRightHook;

        debugReference.action.performed -= Debug;
    }

    // Input action functions
    #region InputActions
    #region BasicMovement
    private void JumpStart(InputAction.CallbackContext context)
    {
        PlayerManager.Instance.movementController.JumpInput = true;
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        PlayerManager.Instance.movementController.JumpInput = false;
    }

    private void ContinuousMove(InputAction.CallbackContext context)
    {
        Vector2 input;
        if (PlayerManager.Instance.allowMovement)
        {
            input = context.ReadValue<Vector2>();

        }
        else
        {
            input = Vector2.zero;
        }

        PlayerManager.Instance.movementController.moveInput = input;
    }
    #endregion
    #region MenuActions
    private void ShowMenu(InputAction.CallbackContext context)
    {
        if(MenuManager.Instance.menuLocked) return;
        GrappleManager.Instance.DisableReticle(0);
        MenuManager.Instance.ShowMenu();
    }

    private void HideMenu(InputAction.CallbackContext context)
    {
        GrappleManager.Instance.EnableReticle(0);
        MenuManager.Instance.HideMenu();
    }
    #endregion
    #region HookActions
    #region LeftHook
    private void FireLeftHook(InputAction.CallbackContext context)
    {
        MenuManager.Instance.HideMenu();
        GrappleManager.Instance.FireHook(0);
    }
    private void ReleaseLeftHook(InputAction.CallbackContext context)
    {
        GrappleManager.Instance.ReleaseHook(0);
    }
    private void ReelOutStartLeftHook(InputAction.CallbackContext context)
    {
        PlayerManager.Instance.grappleController.SetReelingOut(0, true);
    }
    private void ReelOutEndLeftHook(InputAction.CallbackContext context)
    {
        PlayerManager.Instance.grappleController.SetReelingOut(0, false);
    }
    #endregion
    #region RightHook
    private void FireRightHook(InputAction.CallbackContext context)
    {
        GrappleManager.Instance.FireHook(1);
    }
    private void ReleaseRightHook(InputAction.CallbackContext context)
    {
        GrappleManager.Instance.ReleaseHook(1);
    }
    private void ReelOutStartRightHook(InputAction.CallbackContext context)
    {
        PlayerManager.Instance.grappleController.SetReelingOut(1, true);
    }
    private void ReelOutEndRightHook(InputAction.CallbackContext context)
    {
        PlayerManager.Instance.grappleController.SetReelingOut(1, false);
    }
    #endregion

    private void FixedUpdate()
    {
        PlayerManager.Instance.grappleController.SetReelingIn(0, reelInReferenceLeft.action.ReadValue<float>());
        PlayerManager.Instance.grappleController.SetReelingIn(1, reelInReferenceRight.action.ReadValue<float>());

        PlayerManager.Instance.grappleController.SetSwingVelocity(0, swingReferenceLeft.action.ReadValue<Vector3>());
        PlayerManager.Instance.grappleController.SetSwingVelocity(1, swingReferenceRight.action.ReadValue<Vector3>());
    }
    #endregion


    private void Debug(InputAction.CallbackContext context)
    {
        LocationManager.Instance.Debug();
    }
    #endregion
}
