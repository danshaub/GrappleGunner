using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GrappleHook : MonoBehaviour
{
	private bool fired = false;
	private bool hooked = false;
	private bool returning = true;
	private bool retracting = false;

	private float travelSpeed;
	private GrappleGun grappleGun;
    private GrapplePoint.GrappleType lastGrappleType = GrapplePoint.GrappleType.None;

	private Collider cd;
	private Rigidbody rb;
	private Transform returnTransform;
	private float retractInterpolateValue;
	private float snapReturnDistance = 0.5f;

	void Awake()
	{
		gameObject.tag = "Hook";
		cd = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

		gameObject.SetActive(false);
	}

	private void FixedUpdate() {
		if(retracting){
			transform.position = Vector3.Lerp(transform.position, returnTransform.position, retractInterpolateValue);
			transform.rotation = Quaternion.Slerp(transform.rotation, returnTransform.rotation, retractInterpolateValue);

			if(Vector3.Distance(transform.position, returnTransform.position) <= snapReturnDistance){
				FinishRetract();
			}
		}
	}

	void OnCollisionEnter(Collision other) {
        cd.enabled = false;
        if (other.gameObject.tag == "Hookable")
        {
			hooked = true;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.velocity = Vector3.zero;

			GrapplePoint gp = other.gameObject.GetComponent<GrapplePoint>();
			
			if(gp.useRaycastPosition){
				transform.position = other.contacts[0].point;
				transform.rotation = Quaternion.LookRotation(-other.contacts[0].normal, Vector3.up);
			}
			else{
				transform.position = gp.getGrapplePosition();
				transform.rotation = gp.getGrappleRotation();                
            }

            lastGrappleType = gp.type;
            switch (lastGrappleType)
            {
                case GrapplePoint.GrappleType.Red:
                    grappleGun.StartGrappleRed();
                    break;
                case GrapplePoint.GrappleType.Green:
                    grappleGun.StartGrappleGreen();
                    break;
                case GrapplePoint.GrappleType.Blue:
                    grappleGun.StartGrappleBlue();
                    break;
                case GrapplePoint.GrappleType.Orange:
                    grappleGun.StartGrappleOrange();
                    break;
                default:
                    break;
            }
            if (returning)
            {
                ResetHook();
            }
        }
        else
        {
			ResetHook();
        }
	}

	public void SetProperties(float speed, GrappleGun gun, Transform dummyTransform, float interpolateValue){
		travelSpeed = speed;
		grappleGun = gun;
		returnTransform = dummyTransform;
        retractInterpolateValue = interpolateValue;
	}

	public void FireHook(Vector3 pos, Quaternion rot){
		fired = true;
		returning = false;
		cd.enabled = true;
		

		transform.position = pos;
		transform.rotation = rot;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
		rb.velocity = travelSpeed * transform.forward;
	}
	public void ReturnHook(){
		if(hooked){
			ResetHook();
		}
		else{
			returning = true;
		}
	}

	private void ResetHook(){
        cd.enabled = false;
        hooked = false;
		returning = false;

        switch (lastGrappleType)
        {
            case GrapplePoint.GrappleType.Red:
                grappleGun.StopGrappleRed();
                break;
            case GrapplePoint.GrappleType.Green:
                grappleGun.StopGrappleGreen();
                break;
            case GrapplePoint.GrappleType.Blue:
                grappleGun.StopGrappleBlue();
                break;
            case GrapplePoint.GrappleType.Orange:
                grappleGun.StopGrappleOrange();
                break;
            default:
                break;
        }
        cd.enabled = false;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		rb.isKinematic = true;
		rb.detectCollisions = false;
        retracting = true;
    }

	private void FinishRetract(){
		cd.enabled = true;
		retracting = false;
		fired = false;
		rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		rb.detectCollisions = true;

		gameObject.SetActive(false);
	}

	public bool Fired(){
		return fired;
	}
}
