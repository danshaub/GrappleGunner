using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleManager : MonoBehaviour
{
    public static GrappleManager _instance;
    public GrappleCase currentCase;
    public GrappleGun rightGun;
    public GrappleGun leftGun;
    public GrappleOptions options;

    [System.Serializable]
    public enum GrappleCase{
        None,
        SingleRed,
        LeftGreen,
        RightGreen,
        TwoGreen
    }

    public bool allowGrapple = true;

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
        public float limitContactDistance;
        public float swingDamper;
        public float swingVelocityThreshold;
        public float maxSwingVelocity;
        public float swingForceMultiplier;
        public float doubleGreenMultiplier;
        public float snapDistanceMultiplier;
        public float greenReelForce;
        public float groundedReelMultiplier;
        public float greenSnapSpeed;
        public float greenSnapVelocityDamper;
        public AnimationCurve greenSnapVelocityCurve;
        public float greenMaxSlackSpeed;
        public float reelDeadZone;
        public float greenSlackDamper;

        [Header("Reticle Options")]
        public ReticleManager reticleManager;
        public float disabledTransparency;
        public float minReticleDistance;
        public float maxReticleDistance;
        public AnimationCurve reticleScaleCurve;
    }

    private void Awake() {
        Debug.Log("Setting instance");
        if(_instance){
            Destroy(this);
        }
        else{
            _instance = this;
        }
    }

    public void AddRed(){
        currentCase = GrappleCase.SingleRed;
        if(rightGun.hook.state == GrappleHook.GrappleState.Red){
            leftGun.hook.ReturnHook();
        }
        else{
            rightGun.hook.ReturnHook();
        }

        allowGrapple = false;
        PlayerManager._instance.allowMovement = false;
    }

    public void RemoveRed(){
        currentCase = GrappleCase.None;
        allowGrapple = true;
        PlayerManager._instance.allowMovement = true;
    }

    public void AddGreen(bool isLeft){
        switch(currentCase){
            case GrappleCase.None:
                if(isLeft){
                    currentCase = GrappleCase.LeftGreen;
                }
                else{
                    currentCase = GrappleCase.RightGreen;
                }
                break;
            case GrappleCase.LeftGreen:
                currentCase = GrappleCase.TwoGreen;
                break;
            case GrappleCase.RightGreen:
                currentCase = GrappleCase.TwoGreen;
                break;
            default:
                break;
        }
    }
    public void RemoveGreen(bool isLeft){
        switch (currentCase)
        {
            case GrappleCase.LeftGreen:
                currentCase = GrappleCase.None;
                break;
            case GrappleCase.RightGreen:
                currentCase = GrappleCase.None;
                break;
            case GrappleCase.TwoGreen:
                if (isLeft)
                {
                    currentCase = GrappleCase.RightGreen;
                }
                else
                {
                    currentCase = GrappleCase.LeftGreen;
                }
                break;
            default:
                break;
        }   
    }

    public float SwingForceMultiplier(){
        return currentCase == GrappleCase.TwoGreen ? options.doubleGreenMultiplier : options.swingForceMultiplier;
    }
}
