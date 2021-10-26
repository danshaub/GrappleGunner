using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
public class TeleportationManager : MonoBehaviour
{
    // Protects against falling through the floor when teleporting
    public float teleportationOffset = 0.01f;

    // Input actions for teleport buttons
    [SerializeField] private InputActionReference teleportRightHand;
    [SerializeField] private InputActionReference teleportLeftHand;

    // XR Interaction Components
    [SerializeField] private XRRayInteractor rayInteractorRight;
    [SerializeField] private XRRayInteractor rayInteractorLeft;
    [SerializeField] private TeleportationProvider teleportationProvider;

    private void Awake() 
    {    
        // Enable interactions
        rayInteractorRight.enabled = false;
        rayInteractorLeft.enabled = false;

        // Subscribe to necessary events
        teleportRightHand.action.started += OnTeleportActivateRight;
        teleportRightHand.action.canceled += OnTeleportCancelRight;
        teleportLeftHand.action.started += OnTeleportActivateLeft;
        teleportLeftHand.action.canceled += OnTeleportCancelLeft;

    }

    private void OnTeleportActivateRight(InputAction.CallbackContext context)
    {
        // Enable line renderer
        rayInteractorRight.enabled = true;

        // Disable left hand teleporting
        teleportLeftHand.action.started -= OnTeleportActivateLeft;
    }
    private void OnTeleportActivateLeft(InputAction.CallbackContext context)
    {
        // Enable line rendered
        rayInteractorLeft.enabled = true;

        // Disable right hand teleporting
        teleportRightHand.action.started -= OnTeleportActivateRight;
    }

    private void OnTeleportCancelRight(InputAction.CallbackContext context)
    {
        // Check if raycast hits something 
        if(rayInteractorRight.TryGetCurrent3DRaycastHit(out RaycastHit hit)){

            //Create new teleport request for raycast hit position
            TeleportRequest request = new TeleportRequest()
            {
                destinationPosition = hit.point + (Vector3.up * teleportationOffset)
            };

            // Queue teleport request
            teleportationProvider.QueueTeleportRequest(request);
        }

        // Disable line renderer and enable left hand teleporting
        rayInteractorRight.enabled = false;
        teleportLeftHand.action.started += OnTeleportActivateLeft;
    }

    private void OnTeleportCancelLeft(InputAction.CallbackContext context)
    {
        // Check if raycast hits something 
        if(rayInteractorLeft.TryGetCurrent3DRaycastHit(out RaycastHit hit)){
            //Create new teleport request for raycast hit position
            TeleportRequest request = new TeleportRequest()
            {
                destinationPosition = hit.point + (Vector3.up * teleportationOffset)
            };

            // Queue teleport request
            teleportationProvider.QueueTeleportRequest(request);
        }

        // Disable line renderer and enable right hand teleporting
        rayInteractorLeft.enabled = false;
        teleportRightHand.action.started += OnTeleportActivateRight;
    }
}