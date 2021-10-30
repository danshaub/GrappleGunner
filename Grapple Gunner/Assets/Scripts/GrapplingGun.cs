using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// Reference: https://www.youtube.com/watch?v=Xgh4v1w5DxU&t=170s&ab_channel=DanisTutorials
public class GrapplingGun : MonoBehaviour
{
    public float springForce = 4.5f;
    public float damping = 7f;
    public float massScale = 4.5f;

    public float maxDistance = 0.25f;
    public float minDistance = 0.1f;
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public Transform gunTip, player, anchor;
    private SpringJoint joint;

    [HideInInspector] public bool grappling = false;

    public InputActionReference grappleReference = null;
    [SerializeField] private XRRayInteractor rayInteractor;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        grappleReference.action.started += StartGrapple;
        grappleReference.action.canceled += StopGrapple;
    }

    private void LateUpdate() {
        DrawRope();
        if(grappling){
            joint.anchor = anchor.localPosition;
        }
    }

    private void StartGrapple(InputAction.CallbackContext context){
        if(rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)){
            grappling = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            // The distance the grapple will try to keep from grapple point.
            joint.maxDistance = maxDistance;
            joint.minDistance = minDistance;

            joint.spring = springForce;
            joint.damper = damping;
            joint.massScale = massScale;

            joint.anchor = anchor.localPosition;

            lr.positionCount = 2;
        }
    }

    private void DrawRope(){
        if(!joint) return;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    private void StopGrapple(InputAction.CallbackContext context){
        grappling = false;
        lr.positionCount = 0;
        Destroy(joint);
    }
}
