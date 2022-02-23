using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerController playerController;
    private MenuManager menuManager;

    // Basic Input Actions
    public InputActionReference jumpReference = null;
    public InputActionReference moveReference = null;

    // Grapple Actions (Right)
    // Grapple Actions (Left)

    // Menu Actions
    public InputActionReference menuReference = null;

    // REMOVE WITH BUILDS
    public InputActionReference debugReference = null;

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        menuManager = GetComponent<MenuManager>();

        jumpReference.action.started += JumpStart;
        jumpReference.action.canceled += JumpCancel;
        moveReference.action.performed += ContinuousMove;
        moveReference.action.canceled += ContinuousMove;
        menuReference.action.started += ShowMenu;
        menuReference.action.canceled += HideMenu;



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



        debugReference.action.performed -= Debug;
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

        playerController.moveInput = input;
    }

    private void ShowMenu(InputAction.CallbackContext context)
    {
        menuManager.ShowMenu();
    }

    private void HideMenu(InputAction.CallbackContext context)
    {
        menuManager.HideMenu();
    }


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
