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
    public InputActionReference menuAction = null;
    [SerializeField] private PlayerController playerController;
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

        menuAction.action.started += ShowMenu;
        menuAction.action.canceled += HideMenu;
    }

    private void OnDestroy() {
        menuAction.action.started -= ShowMenu;
        menuAction.action.canceled -= HideMenu;
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

        xrRig.transform.position = tpTransform.position;
        xrRig.transform.rotation = tpTransform.rotation;
    }

    private void ShowMenu(InputAction.CallbackContext context)
    {
        menuHandReticle.enabled = false;
        menu.SetActive(true);
        menu.GetComponent<MenuManager>().HomeMenu();
    }

    private void HideMenu(InputAction.CallbackContext context){
        menuHandReticle.enabled = true;
        menu.SetActive(false);
    }
}
