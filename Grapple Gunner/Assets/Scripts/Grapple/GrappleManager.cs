using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleManager : MonoBehaviour
{
    public static GrappleManager _instance;
    public GrappleGun rightGun;
    public GrappleGun leftGun;
    public GrappleOptions options;

    [System.Serializable]
    public struct GrappleOptions{
        [Header("Hook Options")]
        public float hookTravelSpeed;
        public float retractInterpolateValue;
        
        [Header("Red Hook Options")]
        public float redGrappleSpeed;
        public float redVelocityDamper;
        public AnimationCurve redVelocityCurve;

        [Header("Green Hook Options")]
        public float minRopeLength;
        public float swingDamper;
        public float limitContactDistance;
        public float swingVelocityThreshold;
        public float maxSwingVelocity;
        public float swingForceMultiplier;
        public float greenMaxReelSpeed;
        public float greenReelDamper;
        public AnimationCurve greenReelForceCurve;
        public float reelDeadZone;
        public float greenMaxSlackSpeed;
        public float greenSlackDamper;
        public ForceMode forceMode;

        [Header("Reticle Options")]
        public ReticleManager reticleManager;
        public float disabledTransparency;
        public float minReticleDistance;
        public float maxReticleDistance;
        public AnimationCurve reticleScaleCurve;
    }

    private void Awake() {
        if(_instance){
            Destroy(this);
        }
        else{
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
