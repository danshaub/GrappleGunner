using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
public class TeleportationManager : MonoBehaviour
{
    [SerializeField] private InputActionReference teleportRightHand;
    [SerializeField] private InputActionReference teleportLeftHand;

    [SerializeField] private XRRayInteractor rayInteractorRight;
    [SerializeField] private XRRayInteractor rayInteractorLeft;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractorRight.enabled = false;
        rayInteractorLeft.enabled = false;

        teleportRightHand.action.started += OnTeleportActivateRight;
        teleportRightHand.action.canceled += OnTeleportCancelRight;
        teleportLeftHand.action.started += OnTeleportActivateLeft;
        teleportLeftHand.action.canceled += OnTeleportCancelLeft;

    }

    // Update is called once per frame
    void Update()
    {
        //if()
    }

    private void OnTeleportActivateRight(InputAction.CallbackContext context)
    {
        rayInteractorRight.enabled = true;
    }
    private void OnTeleportActivateLeft(InputAction.CallbackContext context)
    {
        rayInteractorLeft.enabled = true;
    }

    private void OnTeleportCancelRight(InputAction.CallbackContext context)
    {
        rayInteractorRight.enabled = false;
    }

    private void OnTeleportCancelLeft(InputAction.CallbackContext context)
    {
        rayInteractorLeft.enabled = false;
    }


}
}
