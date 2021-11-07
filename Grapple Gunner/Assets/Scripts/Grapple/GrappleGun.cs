using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// Reference: https://www.youtube.com/watch?v=Xgh4v1w5DxU&t=170s&ab_channel=DanisTutorials
public class GrappleGun : MonoBehaviour
{
	[Header("Reference Objects")]
	public PlayerManager playerManager;
	public Rigidbody playerRB;
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
	public float redGrappleSpeed = 10f;
	public float redVelocityDamper = 1f;
	public AnimationCurve redVelocityCurve;

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
	[HideInInspector] public bool grappling = false;

	private void Awake() {
		ropeRenderer = GetComponent<LineRenderer>();
		grappleAction.action.started += StartGrapple;
		grappleAction.action.canceled += StopGrapple;

		reticleVisual.SetActive(true);

		originalHookPosition = hook.transform.localPosition;
	}

	private void Start() {
		hook.SetProperties(hookTravelSpeed, this);
	}

	private void Update() {
		if(!hook.gameObject.activeInHierarchy){
			dummyHook.SetActive(true);
            ropeRenderer.positionCount = 0;
            reticleVisual.SetActive(true);
		}
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

	private void FixedUpdate() {
		switch(playerManager.grappleState){
			case PlayerManager.GrappleState.Red:
				HandleRedGrappleMovement();
				break;
			default:
				break;
		}
	}

	private void HandleRedGrappleMovement(){
		playerRB.useGravity = false;

		float distance = Vector3.Distance(ropePoint.position, gunTip.position);
		float multiplier = redVelocityCurve.Evaluate(distance);
		Vector3 targetVelocity = (ropePoint.position - gunTip.position).normalized * redGrappleSpeed * multiplier ;

		float damper = multiplier >= 1 ? redVelocityDamper : 1;
		playerRB.velocity = Vector3.Slerp(playerRB.velocity, targetVelocity, damper);
	}

	#region Start Grapple

	private void StartGrapple(InputAction.CallbackContext context){
		if(playerManager.allowGrapple){
			grappling = true;

			dummyHook.SetActive(false);
			hook.gameObject.SetActive(true);
            reticleVisual.SetActive(false);
            ropeRenderer.positionCount = 2;

			hook.FireHook(dummyHook.transform.position, dummyHook.transform.rotation);
		}
	}

	public void StartGrappleRed()
	{
        Debug.Log("Start Red");
		playerManager.allowGrapple = false;
		playerManager.grappleState = PlayerManager.GrappleState.Red;
	}

	// TODO: Refactor this so it doesn't use the spring joint. Write propriterary joint for this.
	public void StartGrappleGreen()
	{
        Debug.Log("Start Green");
	}

	#endregion

	#region Stop Grapple
	private void StopGrapple(InputAction.CallbackContext context)
	{
		Debug.Log("In grapple gun Stop Grapple");
		hook.ReturnHook();
		grappling = false;
        reticleVisual.SetActive(true);

        ropeRenderer.positionCount = 0;
	}


	public void StopGrappleRed(){
		Debug.Log("Stop Red");
        playerManager.allowGrapple = true;
        playerManager.grappleState = PlayerManager.GrappleState.None;
	}

	public void StopGrappleGreen(){
        Debug.Log("Stop Green");
        playerManager.allowGrapple = true;
        playerManager.grappleState = PlayerManager.GrappleState.None;
	}

	#endregion

	private void DrawRope(){
		if(grappling){
            ropeRenderer.SetPosition(0, gunTip.position);
            ropeRenderer.SetPosition(1, ropePoint.position);
		}
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
