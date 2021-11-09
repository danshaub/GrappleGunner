using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GrappleHook : MonoBehaviour
{
	private bool fired = false;
	private bool hooked = false;
	private bool returned = true;

	private float travelSpeed;
	private GrappleGun grappleGun;
    private GrapplePoint.GrappleType lastGrappleType = GrapplePoint.GrappleType.None;

	private Collider cd;
	private Rigidbody rb;

	void Awake()
	{
		gameObject.tag = "Hook";
		cd = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

		gameObject.SetActive(false);
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

			if(returned){
                Invoke("ResetHook", .5f);
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
        }
        else
        {
			lastGrappleType = GrapplePoint.GrappleType.None;
            rb.constraints = RigidbodyConstraints.None;
			rb.velocity = rb.velocity/10;
			Invoke("ResetHook", .5f);
        }
	}

	public void SetProperties(float speed, GrappleGun gun){
		travelSpeed = speed;
		grappleGun = gun;
	}

	public void FireHook(Vector3 pos, Quaternion rot){
		fired = true;
		returned = false;
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
			returned = true;
		}
	}

	private void ResetHook(){
        cd.enabled = false;
        fired = false;
        hooked = false;
		returned = false;

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
        gameObject.SetActive(false);
    }

	public bool Fired(){
		return fired;
	}

	private IEnumerator DelayColliderActive(){
		if(fired){
			yield return new WaitForSeconds(.01f);
			cd.enabled = fired;
		}

	}
}
