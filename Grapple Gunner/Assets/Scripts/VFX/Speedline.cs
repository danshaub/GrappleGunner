using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedline : MonoBehaviour {
	private float speed;
	private ParticleSystem speedlines;
	public float speedlinesMax;
	private Rigidbody rb;
	
	void Start(){
		rb = PlayerManager.Instance.movementController.rigidbody;
		speedlines = GetComponent<ParticleSystem>();
	}
	
	void FixedUpdate()
	{
		speed = rb.velocity.magnitude;
		if (speed >= speedlinesMax) {
			speedlines.Play();
		}
		gameObject.transform.forward = rb.velocity.normalized;
	}
}