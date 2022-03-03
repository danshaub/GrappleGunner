using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private MenuManager menuManager;

    // Basic Input Actions
    public InputActionReference jumpReference = null;
    public InputActionReference moveReference = null;

    // Grapple Actions (Right)
    public InputActionReference fireReferenceRight = null;
    public InputActionReference reelInReferenceRight = null;
    public InputActionReference reelOutReferenceRight = null;
    // Grapple Actions (Left)
    public InputActionReference fireReferenceLeft = null;
    public InputActionReference reelInReferenceLeft = null;
    public InputActionReference reelOutReferenceLeft = null;

    // Menu Actions
    public InputActionReference menuReference = null;

    // REMOVE WITH BUILDS
    public InputActionReference debugReference = null;

    // Start is called before the first frame update
    void Awake()
    {
        jumpReference.action.started += JumpStart;
        jumpReference.action.canceled += JumpCancel;
        moveReference.action.performed += ContinuousMove;
        moveReference.action.canceled += ContinuousMove;

        menuReference.action.started += ShowMenu;
        menuReference.action.canceled += HideMenu;

        fireReferenceLeft.action.started += FireLeftHook;
        fireReferenceLeft.action.canceled += ReleaseLeftHook;
        reelInReferenceLeft.action.started += ReelInStartLeftHook;
        reelInReferenceLeft.action.canceled += ReelInEndLeftHook;
        reelOutReferenceLeft.action.started += ReelOutStartLeftHook;
        reelOutReferenceLeft.action.canceled += ReelOutEndLeftHook;
        fireReferenceRight.action.started += FireRightHook;
        fireReferenceRight.action.canceled += ReleaseRightHook;
        reelInReferenceRight.action.started += ReelInStartRightHook;
        reelInReferenceRight.action.canceled += ReelInEndRightHook;
        reelOutReferenceRight.action.started += ReelOutStartRightHook;
        reelOutReferenceRight.action.canceled += ReelOutEndRightHook;

        debugReference.action.performed += Debug;
    }

    private void Start()
    {
        menuManager = GetComponent<MenuManager>();
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
        reelInReferenceLeft.action.started -= ReelInStartLeftHook;
        reelInReferenceLeft.action.canceled -= ReelInEndLeftHook;
        reelOutReferenceLeft.action.started -= ReelOutStartLeftHook;
        reelOutReferenceLeft.action.canceled -= ReelOutEndLeftHook;
        fireReferenceRight.action.started -= FireRightHook;
        fireReferenceRight.action.canceled -= ReleaseRightHook;
        reelInReferenceRight.action.started -= ReelInStartRightHook;
        reelInReferenceRight.action.canceled -= ReelInEndRightHook;
        reelOutReferenceRight.action.started -= ReelOutStartRightHook;
        reelOutReferenceRight.action.canceled -= ReelOutEndRightHook;

        debugReference.action.performed -= Debug;
    }

    // Input action functions
    #region InputActions
    #region BasicMovement
    private void JumpStart(InputAction.CallbackContext context)
    {
        PlayerManager._instance.controller.JumpInput = true;
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        PlayerManager._instance.controller.JumpInput = false;
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

        PlayerManager._instance.controller.moveInput = input;
    }
    #endregion
    #region MenuActions
    private void ShowMenu(InputAction.CallbackContext context)
    {
        GrappleManager._instance.DisableReticle(0);
        menuManager.ShowMenu();
    }

    private void HideMenu(InputAction.CallbackContext context)
    {
        GrappleManager._instance.EnableReticle(0);
        menuManager.HideMenu();
    }
    #endregion
    #region HookActions
    #region LeftHook
    private void FireLeftHook(InputAction.CallbackContext context)
    {
        GrappleManager._instance.FireHook(0);
    }
    private void ReleaseLeftHook(InputAction.CallbackContext context)
    {
        GrappleManager._instance.ReleaseHook(0);
    }
    private void ReelInStartLeftHook(InputAction.CallbackContext context)
    {

    }
    private void ReelInEndLeftHook(InputAction.CallbackContext context)
    {

    }
    private void ReelOutStartLeftHook(InputAction.CallbackContext context)
    {

    }
    private void ReelOutEndLeftHook(InputAction.CallbackContext context)
    {

    }
    #endregion
    #region RightHook
    private void FireRightHook(InputAction.CallbackContext context)
    {
        GrappleManager._instance.FireHook(1);
    }
    private void ReleaseRightHook(InputAction.CallbackContext context)
    {
        GrappleManager._instance.ReleaseHook(1);
    }
    private void ReelInStartRightHook(InputAction.CallbackContext context)
    {

    }
    private void ReelInEndRightHook(InputAction.CallbackContext context)
    {

    }
    private void ReelOutStartRightHook(InputAction.CallbackContext context)
    {

    }
    private void ReelOutEndRightHook(InputAction.CallbackContext context)
    {

    }
    #endregion
    #endregion


    private void Debug(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
    }
    #endregion
}
