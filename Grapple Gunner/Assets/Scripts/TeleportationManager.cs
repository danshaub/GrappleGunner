using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
public class TeleportationManager : MonoBehaviour
{
    public float teleportationOffset = 0.01f;
    [SerializeField] private InputActionReference teleportRightHand;
    [SerializeField] private InputActionReference teleportLeftHand;

    [SerializeField] private XRRayInteractor rayInteractorRight;
    [SerializeField] private XRRayInteractor rayInteractorLeft;

    [SerializeField] private TeleportationProvider teleportationProvider;

    private void Awake() 
    {    
        rayInteractorRight.enabled = false;
        rayInteractorLeft.enabled = false;

        teleportRightHand.action.started += OnTeleportActivateRight;
        teleportRightHand.action.canceled += OnTeleportCancelRight;
        teleportLeftHand.action.started += OnTeleportActivateLeft;
        teleportLeftHand.action.canceled += OnTeleportCancelLeft;

    }

    private void OnTeleportActivateRight(InputAction.CallbackContext context)
    {
        rayInteractorRight.enabled = true;
        teleportLeftHand.action.started -= OnTeleportActivateLeft;
        teleportLeftHand.action.canceled -= OnTeleportCancelLeft;
    }
    private void OnTeleportActivateLeft(InputAction.CallbackContext context)
    {
        rayInteractorLeft.enabled = true;
        teleportRightHand.action.started -= OnTeleportActivateRight;
        teleportRightHand.action.canceled -= OnTeleportCancelRight;
    }

    private void OnTeleportCancelRight(InputAction.CallbackContext context)
    {
        

        if(!rayInteractorRight.TryGetCurrent3DRaycastHit(out RaycastHit hit)){
            rayInteractorRight.enabled = false;
            return;
        }

        TeleportRequest request = new TeleportRequest(){
            destinationPosition = hit.point + (Vector3.up * teleportationOffset)
        };

        teleportationProvider.QueueTeleportRequest(request);
        rayInteractorRight.enabled = false;

        teleportLeftHand.action.started += OnTeleportActivateLeft;
        teleportLeftHand.action.canceled += OnTeleportCancelLeft;
    }

    private void OnTeleportCancelLeft(InputAction.CallbackContext context)
    {

        if(!rayInteractorLeft.TryGetCurrent3DRaycastHit(out RaycastHit hit)){
            rayInteractorLeft.enabled = false;
            return;
        }

        TeleportRequest request = new TeleportRequest(){
            destinationPosition = hit.point + (Vector3.up * teleportationOffset)
        };

        teleportationProvider.QueueTeleportRequest(request);
        rayInteractorLeft.enabled = false;

        teleportRightHand.action.started += OnTeleportActivateRight;
        teleportRightHand.action.canceled += OnTeleportCancelRight;
    }
}