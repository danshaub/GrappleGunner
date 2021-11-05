using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// Reference: https://www.youtube.com/watch?v=Xgh4v1w5DxU&t=170s&ab_channel=DanisTutorials
public class GrapplingGun : MonoBehaviour
{
    public PlayerManager PlayerManager;
    public float springForce = 4.5f;
    public float damping = 7f;
    public float massScale = 4.5f;

    public float maxDistance = 0.25f;
    public float minDistance = 0.1f;

    public float minReticleDistance = 5f;
    public float maxReticleDistance = 50f;
    public AnimationCurve reticleScaleCurve;
    private LineRenderer lr;
    private Vector3 grapplePosition;
    public Transform gunTip, player, anchor;
    private SpringJoint joint;

    private GrapplePoint.GrappleType lastGrappleType;

    [HideInInspector] public bool grappling = false;

    public InputActionReference grappleReference = null;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private GameObject reticleVisual;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        grappleReference.action.started += StartGrapple;
        grappleReference.action.canceled += StopGrapple;

        reticleVisual.SetActive(true);
    }

    private void LateUpdate() {
        DrawRope();
        if(grappling && lastGrappleType == GrapplePoint.GrappleType.Green){
            joint.anchor = anchor.localPosition;
        }
        else{
            SetReticle();
        }
    }  

    #region Start Grapple

    private void StartGrapple(InputAction.CallbackContext context){
        if(PlayerManager.allowGrapple){
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                grappling = true;
                grapplePosition = hit.point;

                GrapplePoint grapplePoint = hit.transform.GetComponent<GrapplePoint>();
                lastGrappleType = grapplePoint.type;

                switch (lastGrappleType)
                {
                    case GrapplePoint.GrappleType.Red:
                        StartGrappleRed();
                        break;
                    default:
                        StartGrappleGreen();
                        break;
                }
                
                reticleVisual.SetActive(false);
            }
        }
    }

    private void StartGrappleRed()
    {
        Debug.Log("Red Hit!");
    }

    // TODO: Refactor this so it doesn't use the spring joint. Write propriterary joint for this.
    private void StartGrappleGreen()
    {
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePosition;

        float distanceFromPoint = Vector3.Distance(player.position, grapplePosition);

        // The distance the grapple will try to keep from grapple point.
        joint.maxDistance = maxDistance;
        joint.minDistance = minDistance;

        joint.spring = springForce;
        joint.damper = damping;
        joint.massScale = massScale;

        joint.anchor = anchor.localPosition;

        lr.positionCount = 2;

    }

    #endregion

    #region Stop Grapple
    private void StopGrapple(InputAction.CallbackContext context)
    {
        grappling = false;

        switch(lastGrappleType){
            case GrapplePoint.GrappleType.Red:
                StopGrappleRed();
                break;
            default:
                StopGrappleGreen();
                break;
        }

        reticleVisual.SetActive(true);
    }

    
    private void StopGrappleRed(){
        Debug.Log("Red Stopped!");
    }

    private void StopGrappleGreen(){
        lr.positionCount = 0;
        Destroy(joint);
    }

    #endregion

    private void DrawRope(){
        if(!joint) return;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePosition);
    }

    private void SetReticle(){
        RaycastHit hit;
        if (Physics.Raycast(gunTip.position, gunTip.forward, out hit)){
            float distanceFromPoint = Vector3.Distance(gunTip.position, hit.point);

            float scaledMaxDistance = maxReticleDistance / transform.lossyScale.z;
            float scaledMinDistance = minReticleDistance / transform.lossyScale.z;
            float scaledDistance = distanceFromPoint / transform.lossyScale.z;

            float reticleDistance = Mathf.Clamp(scaledDistance, scaledMinDistance, scaledMaxDistance);

            float reticleScale = reticleScaleCurve.Evaluate((reticleDistance/scaledMaxDistance));

            reticleVisual.transform.localPosition = Vector3.forward * reticleDistance;
            reticleVisual.transform.localScale = new Vector3(reticleScale, reticleScale, 1);
        }
    }
}
