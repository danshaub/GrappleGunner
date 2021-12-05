using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    public GameObject xrRig;
    [SerializeField] private PlayerPhysics playerPhysics;
    [SerializeField] public bool allowMovement = true;
    [SerializeField] public bool grounded = true;
    public float playerHeight = 0;
    public Vector3 playerXZLocalPosistion;
    private XRInputSubsystem subsystem = null;
    public InputActionReference resetViewReference = null;



    private void Awake()
    {
        if (_instance)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        subsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
        resetViewReference.action.performed += ResetViewAction;
    }

    private void Start() {
        ResetView();
    }

    private void ResetViewAction(InputAction.CallbackContext context){
        ResetView();
    }

    public void ResetView(){
        subsystem.TryRecenter();
    }
    public void StopGrounded(){
        playerPhysics.StopGrounded();
    }

    public void TeleportPlayer(Transform tpTransform){
        ResetView();
        playerPhysics.ResetVelocity();

        xrRig.transform.position = tpTransform.position;
        xrRig.transform.rotation = tpTransform.rotation;
    }
}
