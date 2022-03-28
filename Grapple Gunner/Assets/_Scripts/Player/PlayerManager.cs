using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public GameObject player;
    public PlayerMovementController movementController;
    public PlayerGrappleController grappleController;
    public bool allowMovement = true;
    public bool grounded {get {return movementController.isGrounded;} private set {} }
    public bool useGrapplePhysicsMaterial = false;
    public bool useFriction = true;
    public bool useGravity = true;
    public float playerHeight = 0;
    public Vector3 playerXZLocalPosistion;
    private XRInputSubsystem subsystem = null;
    public GameObject menu;
    public MeshRenderer menuHandReticle;


    protected override void Awake()
    {
        base.Awake();
        subsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();

        movementController = player.GetComponent<PlayerMovementController>();
        grappleController = player.GetComponent<PlayerGrappleController>();
    }

    private void Start() {
        ResetView();

        menu.SetActive(false);
    }

    public void ResetView(){
        // subsystem.TryRecenter();
    }

    public void TeleportPlayer(Transform tpTransform){
        ResetView();

        GrappleManager.Instance.ForceReleaseHook();

        movementController.rigidbody.velocity = Vector3.zero;

        player.transform.position = tpTransform.position - playerXZLocalPosistion;
        player.transform.rotation = tpTransform.rotation;
    }
}
