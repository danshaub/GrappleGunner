using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    public GameObject player;
    public PlayerMovementController movementController;
    public PlayerGrappleController grappleController;
    [SerializeField] public bool allowMovement = true;
    [SerializeField] public bool grounded = true;
    public float playerHeight = 0;
    public Vector3 playerXZLocalPosistion;
    private XRInputSubsystem subsystem = null;
    public GameObject menu;
    public MeshRenderer menuHandReticle;


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

        movementController = player.GetComponent<PlayerMovementController>();
        grappleController = player.GetComponent<PlayerGrappleController>();
    }

    private void Start() {
        ResetView();

        menu.SetActive(false);
    }

    public void ResetView(){
        subsystem.TryRecenter();
    }
    public void StopGrounded(){
        // playerPhysics.StopGrounded();
    }

    public void TeleportPlayer(Transform tpTransform){
        ResetView();
        // playerPhysics.ResetVelocity();

        player.transform.position = tpTransform.position;
        player.transform.rotation = tpTransform.rotation;
    }
}
