using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GrappleHook : MonoBehaviour
{
	public float rbDelay = 0.01f;
	private bool fired = false;
	private bool hooked = false;

	[SerializeField] private float travelSpeed;

	private Collider cd;
	private Rigidbody rb;

	void Awake()
	{
		gameObject.tag = "Hook";
		cd = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

		gameObject.SetActive(false);
	}

	// private void FixedUpdate() {
	// 	if(fired && !hooked){
	// 		rb.MovePosition(transform.position + (Vector3.forward * Time.fixedDeltaTime * travelSpeed));
	// 	}
	// }

	void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player")
        {
			Debug.Log("hit player");
            return;
        }
        
        if (other.gameObject.tag == "Hookable")
        {
            hooked = true;
            cd.enabled = false;

			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.velocity = Vector3.zero;

			GrapplePoint gp = other.gameObject.GetComponent<GrapplePoint>();
			if(gp.useRaycastPosition){
				Debug.Log(other.contacts[0].normal);
				transform.position = other.contacts[0].point;
				transform.rotation = Quaternion.LookRotation(-other.contacts[0].normal, Vector3.up);
			}
			else{
				transform.position = gp.getGrapplePosition();
				transform.rotation = gp.getGrappleRotation();
			}
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
			
        }
	}

	public void SetProperties(float speed){
		travelSpeed = speed;
	}

	public void FireHook(Vector3 pos, Quaternion rot){
		fired = true;
		StartCoroutine(DelayColliderActive());
		

		transform.position = pos;
		transform.rotation = rot;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
		rb.velocity = travelSpeed * transform.forward;
	}
	public void ReturnHook(){
		cd.enabled = false;
		fired = false;
		hooked = false;
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
