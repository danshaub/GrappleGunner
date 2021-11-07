using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// Reference: https://www.youtube.com/watch?v=Xgh4v1w5DxU&t=170s&ab_channel=DanisTutorials
public class GrappleGun : MonoBehaviour
{
	[Header("Reference Objects")]
	public PlayerManager PlayerManager;
	public GrappleHook hook;
	public Transform gunTip, player, ropePoint;
	public GameObject dummyHook;
	private LineRenderer ropeRenderer;
	public InputActionReference grappleAction = null;
	[SerializeField] private GameObject reticleVisual;
	[SerializeField] private XRRayInteractor rayInteractor;

	[Header("Hook Options")]
	public float hookTravelSpeed = 40f;

	[Header("Red Hook Options")]
	public float grappleSpeed = 10f;

	[Header("Green Hook Options")]
	public float springForce = 4.5f;
	public float damping = 7f;
	public float massScale = 4.5f;
	public float maxDistance = 0.25f;
	public float minDistance = 0.1f;

	[Header("Reticle Options")]
	public float minReticleDistance = 5f;
	public float maxReticleDistance = 50f;
	public AnimationCurve reticleScaleCurve;

	private Vector3 originalHookPosition;
	private SpringJoint joint;
	private Vector3 grapplePosition;
	private GrapplePoint.GrappleType lastGrappleType;
	[HideInInspector] public bool grappling = false;

	private void Awake() {
		ropeRenderer = GetComponent<LineRenderer>();
		grappleAction.action.started += StartGrapple;
		grappleAction.action.canceled += StopGrapple;

		reticleVisual.SetActive(true);

		originalHookPosition = hook.transform.localPosition;
	}

	private void Start() {
		hook.SetProperties(hookTravelSpeed);
	}

	private void LateUpdate() {
		if(hook.Fired()){
			DrawRope();
		}
		// if(grappling && lastGrappleType == GrapplePoint.GrappleType.Green){
		// 	joint.anchor = anchor.localPosition;
		// }
		else{
			SetReticle();
		}
	}

	#region Start Grapple

	private void StartGrapple(InputAction.CallbackContext context){
		if(PlayerManager.allowGrapple){
			dummyHook.SetActive(false);
			hook.gameObject.SetActive(true);
			hook.FireHook(dummyHook.transform.position, dummyHook.transform.rotation);

            ropeRenderer.positionCount = 2;
			// if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
			// {
			//     grappling = true;
			//     grapplePosition = hit.point;

			//     GrapplePoint grapplePoint = hit.transform.GetComponent<GrapplePoint>();
			//     lastGrappleType = grapplePoint.type;

			//     switch (lastGrappleType)
			//     {
			//         case GrapplePoint.GrappleType.Red:
			//             StartGrappleRed();
			//             break;
			//         default:
			//             StartGrappleGreen();
			//             break;
			//     }

			//     reticleVisual.SetActive(false);
			// }
		}
	}

	private void StartGrappleRed()
	{
		// Debug.Log("Red Hit!");
	}

	// TODO: Refactor this so it doesn't use the spring joint. Write propriterary joint for this.
	private void StartGrappleGreen()
	{
		// joint = player.gameObject.AddComponent<SpringJoint>();
		// joint.autoConfigureConnectedAnchor = false;
		// joint.connectedAnchor = grapplePosition;

		// float distanceFromPoint = Vector3.Distance(player.position, grapplePosition);

		// // The distance the grapple will try to keep from grapple point.
		// joint.maxDistance = maxDistance;
		// joint.minDistance = minDistance;

		// joint.spring = springForce;
		// joint.damper = damping;
		// joint.massScale = massScale;

		// joint.anchor = anchor.localPosition;

	}

	#endregion

	#region Stop Grapple
	private void StopGrapple(InputAction.CallbackContext context)
	{
		grappling = false;

		hook.ReturnHook();
		hook.gameObject.SetActive(false);
		dummyHook.SetActive(true);

        // switch(lastGrappleType){
        // 	case GrapplePoint.GrappleType.Red:
        // 		StopGrappleRed();
        // 		break;
        // 	default:
        // 		StopGrappleGreen();
        // 		break;
        // }

        ropeRenderer.positionCount = 0;
		reticleVisual.SetActive(true);
	}


	private void StopGrappleRed(){
		// Debug.Log("Red Stopped!");
	}

	private void StopGrappleGreen(){
		
		Destroy(joint);
	}

	#endregion

	private void DrawRope(){
		// if(!joint) return;

		ropeRenderer.SetPosition(0, gunTip.position);
		ropeRenderer.SetPosition(1, ropePoint.position);
	}

	private void SetReticle(){
		RaycastHit hit;
		if (Physics.Raycast(gunTip.position, gunTip.forward, out hit)){
			float distanceFromPoint = Vector3.Distance(gunTip.position, hit.point);

			float reticleDistance = Mathf.Clamp(distanceFromPoint, minReticleDistance, maxReticleDistance);
			float reticleScale = reticleScaleCurve.Evaluate((reticleDistance/maxReticleDistance));

			reticleVisual.transform.localPosition = Vector3.forward * reticleDistance;
			reticleVisual.transform.localScale = new Vector3(reticleScale, reticleScale, 1);
		}
	}
}
