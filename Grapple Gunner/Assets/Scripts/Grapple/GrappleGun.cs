using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// Reference: https://www.youtube.com/watch?v=Xgh4v1w5DxU&t=170s&ab_channel=DanisTutorials
public class GrappleGun : MonoBehaviour
{
	public bool isLeft;
	[Header("Reference Objects")]
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

	//Red Hook State Variables
	private bool redLocked;
	private bool redAttemptedRelease;

	//Green Hook state variables
	private ConfigurableJoint joint;
	private float reelInput;
	[HideInInspector] public bool reeling;
	[HideInInspector] public bool slacking;
	[HideInInspector] public float greenCurrentSpoolSpeed;
	private Vector3 swingVelocity; // Velocity of controller in XR rig local space

	//Orange Hook state variables
	private Transform orangeTpTransform;
    private Vector3 orangeTpOffset;
	private GrapplePoint orangePoint;

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
	}

	private void Start() {
        options = GrappleManager._instance.options;
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
		if(hook.fired){
			DrawRope();
		}
		else{
			SetReticle();
		}
	}

	private void FixedUpdate() {
		if(!hook.gameObject.activeInHierarchy){
			return;
		}
		switch(hook.state){
			case GrappleHook.GrappleState.None:
				break;
			case GrappleHook.GrappleState.Red:
                HandleRedGrappleMovement();
				break;
            case GrappleHook.GrappleState.Green:
                HandleGreenGrappleMovement();
                break;
		}
	}

	private void HandleRedGrappleMovement(){
		playerRB.useGravity = false;

		ropeDirection = (gunTip.position - ropePoint.position).normalized;
		float distanceFromPoint = Vector3.Distance(ropePoint.position, gunTip.position);

        redLocked = (distanceFromPoint > PlayerManager._instance.playerHeight *
               		 GrappleManager._instance.options.redUnlockDistanceMultiplier) ||
            		(playerRB.velocity.magnitude > GrappleManager._instance.options.redUnlockVelosityThreshold);

        if (!redLocked && redAttemptedRelease)
        {
            hook.ReturnHook();
            grappling = false;
            reticleVisual.SetActive(true);
        }
        
		float multiplier = GrappleManager._instance.options.redVelocityCurve.Evaluate(distanceFromPoint);
		Vector3 targetVelocity = -ropeDirection *GrappleManager._instance.options. redGrappleSpeed * multiplier ;

		float damper = multiplier >= 1 ? GrappleManager._instance.options.redVelocityDamper : 1;
		playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);
	}

	private void HandleGreenGrappleMovement(){
        joint.anchor = player.InverseTransformPoint(gunTip.position);

        ropeDirection = (gunTip.position - ropePoint.position).normalized;
        float distanceFromPoint = Vector3.Distance(ropePoint.position, gunTip.position);
        SoftJointLimit limit = new SoftJointLimit();
		limit.limit = joint.linearLimit.limit;
        limit.bounciness = 0;
        limit.contactDistance = GrappleManager._instance.options.limitContactDistance;

		bool slackedThisFrame = false;
		bool reeledThisFrame = false;
		bool groundedThisFrame = false;
		
		ApplySwingForce();

		if(reeling && distanceFromPoint > PlayerManager._instance.playerHeight * GrappleManager._instance.options.snapDistanceMultiplier){
            playerRB.AddForce(-ropeDirection * reelInput * GrappleManager._instance.options.greenReelForce);
			if(PlayerManager._instance.grounded) {
				playerRB.AddForce(-ropeDirection * GrappleManager._instance.options.groundedReelMultiplier);
			}
		}
		
        if (PlayerManager._instance.grounded)
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
            limit.limit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
		}
		else if(distanceFromPoint <= PlayerManager._instance.playerHeight * GrappleManager._instance.options.snapDistanceMultiplier){
            float multiplier = GrappleManager._instance.options.greenSnapVelocityCurve.Evaluate(distanceFromPoint);
            Vector3 targetVelocity = -ropeDirection * GrappleManager._instance.options.greenSnapSpeed * multiplier;

            float damper = multiplier >= 1 ? GrappleManager._instance.options.greenSnapVelocityDamper : 1;
            playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);
		}
		else{
			// allow auto retracting, but not slacking
            limit.limit = Mathf.Clamp(distanceFromPoint, PlayerManager._instance.playerHeight - .05f, joint.linearLimit.limit);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;

			playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, Vector3.zero, GrappleManager._instance.options.swingDamper);
		}

		if(!reeledThisFrame && !slackedThisFrame){
            greenCurrentSpoolSpeed = Mathf.Lerp(greenCurrentSpoolSpeed, 0, GrappleManager._instance.options.greenSlackDamper);

			if(!groundedThisFrame && greenCurrentSpoolSpeed >= 0.001){
                limit.limit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);
			}
		}

        joint.linearLimit = limit;
	}
	private void ApplySwingForce(){

		swingVelocity = player.TransformVector(gunVelocity.action.ReadValue<Vector3>()); //Quaternion.Euler(0, player.eulerAngles.y, 0) * gunVelocity.action.ReadValue<Vector3>();
		
		float swingMagnitude = Vector3.Dot(swingVelocity, ropeDirection);
		swingMagnitude = Mathf.Clamp(swingMagnitude, GrappleManager._instance.options.swingVelocityThreshold, GrappleManager._instance.options.maxSwingVelocity);
		if(swingMagnitude > GrappleManager._instance.options.swingVelocityThreshold){
            playerRB.AddForce(-ropeDirection * swingMagnitude * GrappleManager._instance.SwingForceMultiplier());
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

	private void OrangeTeleport(){
		StartCoroutine(TPOnFixedUpdate());
	}

	private IEnumerator TPOnFixedUpdate(){
        Vector3 tempPosition = player.position + PlayerManager._instance.playerXZLocalPosistion;
        playerRB.MovePosition(orangeTpTransform.position + orangeTpOffset - PlayerManager._instance.playerXZLocalPosistion);
        orangeTpTransform.position = tempPosition - orangeTpOffset;
		PlayerManager._instance.StopGrounded();

        orangePoint.DecrementUses();

        hook.ReturnHook(true);

		yield return new WaitForFixedUpdate();		
		yield return new WaitForFixedUpdate();

        try
        {
            Rigidbody pointRB = orangePoint.GetComponent<Rigidbody>();
            Vector3 pointVelocity = pointRB.velocity;
            pointRB.velocity = playerRB.velocity;
            playerRB.velocity = pointVelocity;
        }
        catch { }

        orangeTpTransform = null;
        orangeTpOffset = Vector3.zero;
        orangePoint = null;

        GrappleManager._instance.RemoveOrange(isLeft);
	}

	#region Start Grapple

	private void StartGrapple(InputAction.CallbackContext context){
		redAttemptedRelease = false;
		if(GrappleManager._instance.allowGrapple && !hook.fired){
			dummyHook.SetActive(false);
			hook.gameObject.SetActive(true);
            reticleVisual.SetActive(false);
            ropeRenderer.positionCount = 2;

			hook.FireHook(dummyHook.transform.position, dummyHook.transform.rotation);
		}
	}

	public void StartGrappleRed()
	{
		GrappleManager._instance.AddRed();
		redLocked = true;
	}

	// TODO: Refactor this so it doesn't use the spring joint. Write propriterary joint for this.
	public void StartGrappleGreen()
	{
        GrappleManager._instance.AddGreen(isLeft);

        joint = player.gameObject.AddComponent<ConfigurableJoint>();
		joint.autoConfigureConnectedAnchor = false;
		joint.anchor = player.InverseTransformPoint(gunTip.position);
        // joint.anchor = player.InverseTransformPoint(vrCamera.position);
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

    public void StartGrappleOrange(Transform tpTransform, Vector3 tpOffset, GrapplePoint point)
    {
		GrappleManager._instance.AddOrange(isLeft);
        orangeTpTransform = tpTransform;
		orangeTpOffset = tpOffset;
		orangePoint = point;

		Invoke("OrangeTeleport", GrappleManager._instance.options.orangeTimeToTeleport);
    }

	#endregion

	#region Stop Grapple
	private void StopGrapple(InputAction.CallbackContext context)
	{
		if(!redLocked){
            hook.ReturnHook();
            grappling = false;
            reticleVisual.SetActive(true);
		}
		else{
			redAttemptedRelease = true;
		}
	}


	public void StopGrappleRed(){
        GrappleManager._instance.RemoveRed();
	}

	public void StopGrappleGreen(){
		GrappleManager._instance.RemoveGreen(isLeft);
		Destroy(joint);
	}

    public void StopGrappleBlue()
    {
        Debug.Log("Stop Blue");
        // PlayerManager._instance.allowGrapple = true;
        // PlayerManager._instance.grappleState = PlayerManager.GrappleState.None;
    }

    public void StopGrappleOrange()
    {
        orangeTpTransform = null;
        orangeTpOffset = Vector3.zero;

		GrappleManager._instance.RemoveOrange(isLeft);

        CancelInvoke("OrangeTeleport");
    }

	#endregion

	private void DrawRope(){
		ropeRenderer.SetPosition(0, gunTip.position);
		ropeRenderer.SetPosition(1, ropePoint.position);
	}

	private void SetReticle(){
		RaycastHit hit;
		if (Physics.Raycast(gunTip.position, gunTip.forward, out hit)){
			if(hit.transform.gameObject.tag == "Hookable" && GrappleManager._instance.allowGrapple){
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
					case GrapplePoint.GrappleType.OrangeDisabled:
						reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.disabled);
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
