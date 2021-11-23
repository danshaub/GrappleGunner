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
	public Transform gunTip, player, ropePoint, vrCamera;
	public GameObject dummyHook;
	private LineRenderer ropeRenderer;
	public InputActionReference grappleAction = null;
	public InputActionReference reelAction = null;
    public InputActionReference slackAction = null;
	public InputActionReference gunVelocity = null;
	[SerializeField] private GameObject reticleVisual;
	[SerializeField] private XRRayInteractor rayInteractor;
	private GrappleManager.GrappleOptions options;
	
	//Hook State Variables
	[HideInInspector] public bool grappling = false;
	private Vector3 originalHookPosition;
	private Vector3 ropeDirection; //From gun tip to hook

	//Green Hook state variables
	// public Vector3 greenReelForce;
	// [HideInInspector] public float greenReelDamperClamped;
	private ConfigurableJoint joint;
	private float reelInput;
	[HideInInspector] public bool reeling;
	[HideInInspector] public bool slacking;
	[HideInInspector] public float greenCurrentSpoolSpeed;
	private Vector3 swingVelocity; // Velocity of controller in XR rig local space

	//Reticle State Variables
	private Material reticleMaterial;


	private void Awake() {
		ropeRenderer = GetComponent<LineRenderer>();
		grappleAction.action.started += StartGrapple;
		grappleAction.action.canceled += StopGrapple;
		slackAction.action.started += Slack;
		slackAction.action.canceled += EndSlack;

		reticleVisual.SetActive(true);

		originalHookPosition = hook.transform.localPosition;
		options = GrappleManager._instance.options;
	}

	private void Start() {
		hook.SetProperties(GrappleManager._instance.options.hookTravelSpeed, this, dummyHook.transform, GrappleManager._instance.options.retractInterpolateValue);
        reticleMaterial = reticleVisual.GetComponent<Renderer>().material;

		ropeRenderer.enabled = true;
		hook.gameObject.SetActive(true);
	}

	private void Update() {
		if(!hook.gameObject.activeInHierarchy){
			dummyHook.SetActive(true);
            ropeRenderer.positionCount = 0;
            reticleVisual.SetActive(true);
		}

		if(joint){
			reelInput = reelAction.action.ReadValue<float>();
			reeling = reelInput > GrappleManager._instance.options.reelDeadZone;

		}
		else{
			reeling = false;
		}
	}

	private void LateUpdate() {
		if(hook.Fired()){
			DrawRope();
		}
		else{
			SetReticle();
		}
	}

	private void FixedUpdate() {
		if(playerManager.grappleState == PlayerManager.GrappleState.Red){
            HandleRedGrappleMovement();
		}
		else if(joint){
            HandleGreenGrappleMovement();
		}
	}

	private void HandleRedGrappleMovement(){
		Debug.Log("HERE");
		ropeDirection = (gunTip.position - ropePoint.position).normalized;
		playerRB.useGravity = false;

		float distance = Vector3.Distance(ropePoint.position, gunTip.position);
		float multiplier = GrappleManager._instance.options.redVelocityCurve.Evaluate(distance);
		Vector3 targetVelocity = -ropeDirection *GrappleManager._instance.options. redGrappleSpeed * multiplier ;

		float damper = multiplier >= 1 ? GrappleManager._instance.options.redVelocityDamper : 1;
		playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);
	}

	private void HandleGreenGrappleMovement(){
        joint.anchor = player.InverseTransformPoint(vrCamera.position);

        float distanceFromPoint = Vector3.Distance(ropePoint.position, vrCamera.position);
        SoftJointLimit limit = new SoftJointLimit();
		limit.limit = joint.linearLimit.limit;
        limit.bounciness = 0;
        limit.contactDistance = GrappleManager._instance.options.limitContactDistance;

		bool slackedThisFrame = false;
		bool reeledThisFrame = false;
		bool groundedThisFrame = false;
		
		ApplySwingForce();

		if (reeling){
			reeledThisFrame = true;

            greenCurrentSpoolSpeed = Mathf.Lerp(greenCurrentSpoolSpeed, -GrappleManager._instance.options.greenMaxReelSpeed, GrappleManager._instance.options.greenReelDamper);
            float newLimit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);
            limit.limit = Mathf.Clamp(newLimit, GrappleManager._instance.options.minRopeLength, float.MaxValue); 

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
		}
        else if (playerManager.grounded)
        {
            groundedThisFrame = true;

            limit.limit = distanceFromPoint;

			greenCurrentSpoolSpeed = 0f;

            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
        }
		else if(slacking){
			slackedThisFrame = true;
			greenCurrentSpoolSpeed = Mathf.Lerp(greenCurrentSpoolSpeed, GrappleManager._instance.options.greenMaxSlackSpeed, GrappleManager._instance.options.greenSlackDamper);
            float newLimit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);
            limit.limit = Mathf.Clamp(newLimit, GrappleManager._instance.options.minRopeLength, float.MaxValue);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
		}
		else{
			// allow auto retracting, but not slacking
            limit.limit = Mathf.Clamp(distanceFromPoint, GrappleManager._instance.options.minRopeLength, joint.linearLimit.limit);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;

			playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, Vector3.zero, GrappleManager._instance.options.swingDamper);
		}

		if(!reeledThisFrame && !slackedThisFrame){
            greenCurrentSpoolSpeed = Mathf.Lerp(greenCurrentSpoolSpeed, 0, GrappleManager._instance.options.greenReelDamper + GrappleManager._instance.options.greenSlackDamper);

			if(!groundedThisFrame && greenCurrentSpoolSpeed >= 0.001){
                float newLimit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);
                limit.limit = Mathf.Clamp(newLimit, GrappleManager._instance.options.minRopeLength, float.MaxValue);
			}
		}

        joint.linearLimit = limit;
	}
	private void ApplySwingForce(){
		ropeDirection = (gunTip.position - ropePoint.position).normalized;
		swingVelocity = player.TransformVector(gunVelocity.action.ReadValue<Vector3>()); //Quaternion.Euler(0, player.eulerAngles.y, 0) * gunVelocity.action.ReadValue<Vector3>();
		
		float swingMagnitude = Vector3.Dot(swingVelocity, ropeDirection);
		swingMagnitude = Mathf.Clamp(swingMagnitude, GrappleManager._instance.options.swingVelocityThreshold, GrappleManager._instance.options.maxSwingVelocity);
		if(swingMagnitude > GrappleManager._instance.options.swingVelocityThreshold){
			// playerRB.AddForce(-ropeDirection * swingMagnitude * swingForceMultiplier);
            playerRB.AddForce(-(swingVelocity + ropeDirection).normalized * swingMagnitude * GrappleManager._instance.options.swingForceMultiplier);
		}
	}
    private void Slack(InputAction.CallbackContext context)
    {
		slacking = true;
    }
    private void EndSlack(InputAction.CallbackContext context)
    {
		slacking = false;
    }

	#region Start Grapple

	private void StartGrapple(InputAction.CallbackContext context){
		if(playerManager.allowGrapple && !hook.Fired()){
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
		playerManager.allowGrapple = false;
		playerManager.grappleState = PlayerManager.GrappleState.Red;
	}

	// TODO: Refactor this so it doesn't use the spring joint. Write propriterary joint for this.
	public void StartGrappleGreen()
	{
        joint = player.gameObject.AddComponent<ConfigurableJoint>();
		joint.autoConfigureConnectedAnchor = false;
		// joint.anchor = player.InverseTransformPoint(gunTip.position);
        joint.anchor = player.InverseTransformPoint(vrCamera.position);
		joint.connectedAnchor = ropePoint.position;

		float distanceFromPoint = Vector3.Distance(gunTip.position, ropePoint.position);

		SoftJointLimit limit = new SoftJointLimit();
		limit.limit = distanceFromPoint;
		limit.bounciness = 0;
        limit.contactDistance = GrappleManager._instance.options.limitContactDistance;
		joint.linearLimit = limit;

		greenCurrentSpoolSpeed = 0;
	}

    public void StartGrappleBlue()
    {
        Debug.Log("Start Blue");
    }

    public void StartGrappleOrange()
    {
        Debug.Log("Start Orange");
    }

	#endregion

	#region Stop Grapple
	private void StopGrapple(InputAction.CallbackContext context)
	{
		hook.ReturnHook();
		grappling = false;
        reticleVisual.SetActive(true);
	}


	public void StopGrappleRed(){
        playerManager.allowGrapple = true;
        playerManager.grappleState = PlayerManager.GrappleState.None;
	}

	public void StopGrappleGreen(){
        CancelInvoke("ActivateSwingForceCooldown");
        CancelInvoke("ResetSwingForce");
		Destroy(joint);
	}

    public void StopGrappleBlue()
    {
        Debug.Log("Stop Blue");
        // playerManager.allowGrapple = true;
        playerManager.grappleState = PlayerManager.GrappleState.None;
    }

    public void StopGrappleOrange()
    {
        Debug.Log("Stop Orange");
        // playerManager.allowGrapple = true;
        playerManager.grappleState = PlayerManager.GrappleState.None;
    }

	#endregion

	private void DrawRope(){
		if(hook.Fired()){
            ropeRenderer.SetPosition(0, gunTip.position);
            ropeRenderer.SetPosition(1, ropePoint.position);
		}
	}

	private void SetReticle(){
		RaycastHit hit;
		if (Physics.Raycast(gunTip.position, gunTip.forward, out hit)){
			if(hit.transform.gameObject.tag == "Hookable"){
				GrapplePoint.GrappleType type = hit.transform.gameObject.GetComponent<GrapplePoint>().type;
				reticleMaterial.SetFloat("_Transparency", 1f);
				switch(type){
					case GrapplePoint.GrappleType.Red:
						reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.red);
						break;
                    case GrapplePoint.GrappleType.Green:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.green);
                        break;
                    case GrapplePoint.GrappleType.Blue:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.blue);
                        break;
                    case GrapplePoint.GrappleType.Orange:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.orange);
                        break;
				}
			}
			else{
                reticleMaterial.SetFloat("_Transparency", GrappleManager._instance.options.disabledTransparency);
                reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.disabled);
			}

			float distanceFromPoint = Vector3.Distance(gunTip.position, hit.point);

			float reticleDistance = Mathf.Clamp(distanceFromPoint, GrappleManager._instance.options.minReticleDistance, GrappleManager._instance.options.maxReticleDistance);
			float reticleScale = GrappleManager._instance.options.reticleScaleCurve.Evaluate((reticleDistance/GrappleManager._instance.options.maxReticleDistance));

			reticleVisual.transform.localPosition = Vector3.forward * reticleDistance;
			reticleVisual.transform.localScale = new Vector3(reticleScale, reticleScale, 1);
		}
	}
}
