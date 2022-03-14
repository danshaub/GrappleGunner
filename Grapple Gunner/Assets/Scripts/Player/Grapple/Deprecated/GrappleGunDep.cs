using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// Reference: https://www.youtube.com/watch?v=Xgh4v1w5DxU&t=170s&ab_channel=DanisTutorials
public class GrappleGunDep : MonoBehaviour
{
    public bool isLeft;
    [Header("Reference Objects")]
    public Rigidbody playerRB;
    public GrappleHookDep hook;
    public Transform gunTip, player, ropePoint, vrCamera;
    public GameObject dummyHook;
    private LineRenderer ropeRenderer;
    public InputActionReference grappleAction = null;
    public InputActionReference reelAction = null;
    public InputActionReference slackAction = null;
    public InputActionReference gunVelocity = null;
    [SerializeField] private GameObject reticleVisual;
    [SerializeField] private XRRayInteractor rayInteractor;
    private GrappleManagerDep.GrappleOptions options;

    //Hook State Variables
    [HideInInspector] public bool grappling = false;
    private Vector3 originalHookPosition;
    private Vector3 ropeDirection; //From gun tip to hook

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
    private GrapplePointDep orangePoint;

    //Reticle State Variables
    private Material reticleMaterial;


    private void Awake()
    {
        ropeRenderer = GetComponent<LineRenderer>();
        grappleAction.action.started += StartGrapple;
        grappleAction.action.canceled += StopGrapple;
        slackAction.action.started += Slack;
        slackAction.action.canceled += EndSlack;

        reticleVisual.SetActive(true);

        originalHookPosition = hook.transform.localPosition;
    }

    private void OnDestroy() {
        grappleAction.action.started -= StartGrapple;
        grappleAction.action.canceled -= StopGrapple;
        slackAction.action.started -= Slack;
        slackAction.action.canceled -= EndSlack;
    }

    private void Start()
    {
        options = GrappleManagerDep._instance.options;
        hook.SetProperties(GrappleManagerDep._instance.options.hookTravelSpeed, this, dummyHook.transform, GrappleManagerDep._instance.options.retractInterpolateValue);
        reticleMaterial = reticleVisual.GetComponent<Renderer>().material;

        ropeRenderer.enabled = true;
        hook.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!hook.gameObject.activeInHierarchy)
        {
            dummyHook.SetActive(true);
            ropeRenderer.positionCount = 0;
            reticleVisual.SetActive(true);
        }

        if (joint)
        {
            reelInput = reelAction.action.ReadValue<float>();
            reeling = reelInput > GrappleManagerDep._instance.options.reelDeadZone;

        }
        else
        {
            reeling = false;
        }
    }

    private void LateUpdate()
    {
        if (hook.fired)
        {
            DrawRope();
        }
        else
        {
            SetReticle();
        }
    }

    private void FixedUpdate()
    {
        if (!hook.gameObject.activeInHierarchy)
        {
            return;
        }
        switch (hook.state)
        {
            case GrappleHookDep.GrappleState.None:
                break;
            case GrappleHookDep.GrappleState.Red:
                HandleRedGrappleMovement();
                break;
            case GrappleHookDep.GrappleState.Green:
                HandleGreenGrappleMovement();
                break;
        }
    }

    private void HandleRedGrappleMovement()
    {
        playerRB.useGravity = false;

        ropeDirection = (gunTip.position - ropePoint.position).normalized;
        float distanceFromPoint = Vector3.Distance(ropePoint.position, gunTip.position);

        float multiplier = GrappleManagerDep._instance.options.redVelocityCurve.Evaluate(distanceFromPoint);
        Vector3 targetVelocity = -ropeDirection * GrappleManagerDep._instance.options.redGrappleSpeed * multiplier;

        float damper = multiplier >= 1 ? GrappleManagerDep._instance.options.redVelocityDamper : 1;
        playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);
    }

    private void HandleGreenGrappleMovement()
    {
        joint.anchor = player.InverseTransformPoint(gunTip.position);

        ropeDirection = (gunTip.position - ropePoint.position).normalized;
        float distanceFromPoint = Vector3.Distance(ropePoint.position, gunTip.position);
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = joint.linearLimit.limit;
        limit.bounciness = 0;
        limit.contactDistance = GrappleManagerDep._instance.options.limitContactDistance;

        bool slackedThisFrame = false;
        bool reeledThisFrame = false;
        bool groundedThisFrame = false;

        ApplySwingForce();

        if (reeling && distanceFromPoint > PlayerManager.Instance.playerHeight * GrappleManagerDep._instance.options.snapDistanceMultiplier)
        {
            playerRB.AddForce(-ropeDirection * reelInput * GrappleManagerDep._instance.options.greenReelForce);
            if (PlayerManager.Instance.grounded)
            {
                playerRB.AddForce(-ropeDirection * GrappleManagerDep._instance.options.groundedReelMultiplier);
            }
        }

        if (PlayerManager.Instance.grounded)
        {
            groundedThisFrame = true;

            limit.limit = distanceFromPoint;

            greenCurrentSpoolSpeed = 0f;

            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
        }
        else if (slacking)
        {
            slackedThisFrame = true;
            greenCurrentSpoolSpeed = Mathf.Lerp(greenCurrentSpoolSpeed, GrappleManagerDep._instance.options.greenMaxSlackSpeed, GrappleManagerDep._instance.options.greenSlackDamper);
            limit.limit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
        }
        else if (distanceFromPoint <= PlayerManager.Instance.playerHeight * GrappleManagerDep._instance.options.snapDistanceMultiplier)
        {
            float multiplier = GrappleManagerDep._instance.options.greenSnapVelocityCurve.Evaluate(distanceFromPoint);
            Vector3 targetVelocity = -ropeDirection * GrappleManagerDep._instance.options.greenSnapSpeed * multiplier;

            float damper = multiplier >= 1 ? GrappleManagerDep._instance.options.greenSnapVelocityDamper : 1;
            playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, targetVelocity, damper);
        }
        else
        {
            // allow auto retracting, but not slacking
            limit.limit = Mathf.Clamp(distanceFromPoint, PlayerManager.Instance.playerHeight - .05f, joint.linearLimit.limit);

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;

            playerRB.velocity = Vector3.LerpUnclamped(playerRB.velocity, Vector3.zero, GrappleManagerDep._instance.options.swingDamper);
        }

        if (!reeledThisFrame && !slackedThisFrame)
        {
            greenCurrentSpoolSpeed = Mathf.Lerp(greenCurrentSpoolSpeed, 0, GrappleManagerDep._instance.options.greenSlackDamper);

            if (!groundedThisFrame && greenCurrentSpoolSpeed >= 0.001)
            {
                limit.limit = joint.linearLimit.limit + (greenCurrentSpoolSpeed * Time.deltaTime);
            }
        }

        joint.linearLimit = limit;
    }
    private void ApplySwingForce()
    {

        swingVelocity = player.TransformVector(gunVelocity.action.ReadValue<Vector3>()); //Quaternion.Euler(0, player.eulerAngles.y, 0) * gunVelocity.action.ReadValue<Vector3>();

        float swingMagnitude = Vector3.Dot(swingVelocity, ropeDirection);
        swingMagnitude = Mathf.Clamp(swingMagnitude, GrappleManagerDep._instance.options.swingVelocityThreshold, GrappleManagerDep._instance.options.maxSwingVelocity);
        if (swingMagnitude > GrappleManagerDep._instance.options.swingVelocityThreshold)
        {
            playerRB.AddForce(-ropeDirection * swingMagnitude * GrappleManagerDep._instance.SwingForceMultiplier());
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

    private void OrangeTeleport()
    {
        StartCoroutine(TPOnFixedUpdate());
    }

    private IEnumerator TPOnFixedUpdate()
    {
        Vector3 tempPosition = player.position + PlayerManager.Instance.playerXZLocalPosistion;
        playerRB.MovePosition(orangeTpTransform.position + orangeTpOffset - PlayerManager.Instance.playerXZLocalPosistion);
        orangeTpTransform.eulerAngles = new Vector3(0f, orangeTpTransform.eulerAngles.y, 0f);
        orangeTpTransform.position = tempPosition - orangeTpOffset;
        PlayerManager.Instance.StopGrounded();

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

        GrappleManagerDep._instance.RemoveOrange(isLeft);
    }

    #region Start Grapple

    private void StartGrapple(InputAction.CallbackContext context)
    {
        if (GrappleManagerDep._instance.allowGrapple && !hook.fired)
        {
            dummyHook.SetActive(false);
            hook.gameObject.SetActive(true);
            reticleVisual.SetActive(false);
            ropeRenderer.positionCount = 2;

            hook.FireHook(dummyHook.transform.position, dummyHook.transform.rotation);
        }
    }

    public void StartGrappleRed()
    {
        GrappleManagerDep._instance.AddRed();
    }

    public void StartGrappleGreen()
    {
        GrappleManagerDep._instance.AddGreen(isLeft);

        joint = player.gameObject.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = player.InverseTransformPoint(gunTip.position);
        // joint.anchor = player.InverseTransformPoint(vrCamera.position);
        joint.connectedAnchor = ropePoint.position;

        float distanceFromPoint = Vector3.Distance(gunTip.position, ropePoint.position);

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = distanceFromPoint;
        limit.bounciness = 0;
        limit.contactDistance = GrappleManagerDep._instance.options.limitContactDistance;
        joint.linearLimit = limit;

        greenCurrentSpoolSpeed = 0;
    }

    public void StartGrappleBlue()
    {
        Debug.Log("Start Blue");
    }

    public void StartGrappleOrange(Transform tpTransform, Vector3 tpOffset, GrapplePointDep point)
    {
        GrappleManagerDep._instance.AddOrange(isLeft);
        orangeTpTransform = tpTransform;
        orangeTpOffset = tpOffset;
        orangePoint = point;

        Invoke("OrangeTeleport", GrappleManagerDep._instance.options.orangeTimeToTeleport);
    }

    #endregion

    #region Stop Grapple
    private void StopGrapple(InputAction.CallbackContext context)
    {
        hook.ReturnHook();
        grappling = false;
        reticleVisual.SetActive(true);
    }


    public void StopGrappleRed()
    {
        GrappleManagerDep._instance.RemoveRed();
        playerRB.velocity = Vector3.zero;
    }

    public void StopGrappleGreen()
    {
        Destroy(joint);
        GrappleManagerDep._instance.RemoveGreen(isLeft);
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

        GrappleManagerDep._instance.RemoveOrange(isLeft);

        CancelInvoke("OrangeTeleport");
    }

    #endregion

    private void DrawRope()
    {
        ropeRenderer.SetPosition(0, gunTip.position);
        ropeRenderer.SetPosition(1, ropePoint.position);
    }

    private void SetReticle()
    {
        if(!reticleVisual.activeInHierarchy){
            return;
        }
        
        RaycastHit hit;
        // if (Physics.Raycast(gunTip.position, gunTip.forward, out hit))
        if (Physics.SphereCast(gunTip.position, GrappleManagerDep._instance.options.sphereCastRadius,
                               gunTip.forward, out hit, 2000, GrappleManagerDep._instance.options.sphereCastMask))
        {
            if (hit.transform.gameObject.tag == "Hookable" && GrappleManagerDep._instance.allowGrapple)
            {
                GrapplePointDep.GrappleType type = hit.transform.gameObject.GetComponent<GrapplePointDep>().type;
                reticleMaterial.SetFloat("_Transparency", 1f);
                switch (type)
                {
                    case GrapplePointDep.GrappleType.Red:
                        reticleMaterial.SetTexture("_MainTex", GrappleManagerDep._instance.options.reticleManager.red);
                        break;
                    case GrapplePointDep.GrappleType.Green:
                        reticleMaterial.SetTexture("_MainTex", GrappleManagerDep._instance.options.reticleManager.green);
                        break;
                    case GrapplePointDep.GrappleType.Blue:
                        reticleMaterial.SetTexture("_MainTex", GrappleManagerDep._instance.options.reticleManager.blue);
                        break;
                    case GrapplePointDep.GrappleType.Orange:
                        reticleMaterial.SetTexture("_MainTex", GrappleManagerDep._instance.options.reticleManager.orange);
                        break;
                    case GrapplePointDep.GrappleType.OrangeDisabled:
                        reticleMaterial.SetTexture("_MainTex", GrappleManagerDep._instance.options.reticleManager.disabled);
                        break;
                    case GrapplePointDep.GrappleType.Button:
                        reticleMaterial.SetTexture("_MainTex", GrappleManagerDep._instance.options.reticleManager.button);
                        break;
                }
            }
            else
            {
                reticleMaterial.SetFloat("_Transparency", GrappleManagerDep._instance.options.disabledTransparency);
                reticleMaterial.SetTexture("_MainTex", GrappleManagerDep._instance.options.reticleManager.disabled);
            }

            float distanceFromPoint = Vector3.Distance(gunTip.position, hit.point);

            float reticleDistance = Mathf.Clamp(distanceFromPoint, GrappleManagerDep._instance.options.minReticleDistance, GrappleManagerDep._instance.options.maxReticleDistance);
            float reticleScale = GrappleManagerDep._instance.options.reticleScaleCurve.Evaluate((reticleDistance / GrappleManagerDep._instance.options.maxReticleDistance));

            reticleVisual.transform.localPosition = Vector3.forward * reticleDistance;
            reticleVisual.transform.localScale = new Vector3(reticleScale, reticleScale, 1);
        }
    }
}