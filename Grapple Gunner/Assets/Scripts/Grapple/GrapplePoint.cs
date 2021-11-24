using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class GrapplePoint : MonoBehaviour
{
	//Defines which kind of grapple interaction the grapple point will have.
	public enum GrappleType
	{
		Red,
		Green,
		Blue,
		Orange,
		OrangeDisabled,
		UI,
		None
	}


	public GrappleType type;
	public bool useRaycastPosition = false;
	public Vector3 grapplePosition;
	public Vector3 grappleRotation;

	[Header("Options for Blue Point")]
	public Rigidbody blueGrabRB;

	[Header("Options for Orange Point")]
	public Transform teleportParent;
	public Vector3 teleportOffset;
	public Material disabledMaterial;
	public bool infiniteUses = true;
	public int numberUses;
	private int remainingUses;

	[Header("Gizmo Mesh")]
	public Mesh grappleMesh;
	private Vector3 teleportCube = new Vector3(.1f,.1f,.1f);
	private Material originalMaterial;

	public Vector3 getGrapplePosition(){
		return transform.position + grapplePosition;
	}
	public Quaternion getGrappleRotation(){
		return Quaternion.Euler(grappleRotation);
	}

	private void Start() {
		gameObject.tag = "Hookable";
		originalMaterial = GetComponent<MeshRenderer>().material;
		remainingUses = numberUses;
	}

	public void DecrementUses(){
		if(infiniteUses){
			return;
		}
		else{
			remainingUses--;
			if(remainingUses == 0){
				type = GrappleType.OrangeDisabled;
				GetComponent<MeshRenderer>().material = disabledMaterial;
			}
		}

	}


	#if UNITY_EDITOR
	private void OnDrawGizmos() {
		if(Application.isPlaying){
			return;
		}
		switch(type){
			case GrappleType.Red: 
				Gizmos.color = Color.red;
				break;
			case GrappleType.Green:
				Gizmos.color = Color.green;
				break;
			case GrappleType.Blue:
				if(!blueGrabRB){
					Gizmos.color = Color.red;
					Gizmos.DrawWireSphere(transform.position, transform.lossyScale.magnitude);
				}
				Gizmos.color = Color.blue;
				break;
			case GrappleType.Orange:
				Gizmos.color = Color.yellow;
				Gizmos.DrawCube((teleportParent.position + teleportOffset), teleportCube);
				break;
		}
		if (useRaycastPosition){
			Gizmos.DrawWireMesh(GetComponent<MeshFilter>().sharedMesh,transform.position, transform.rotation, transform.lossyScale);
		}
		else{
            Vector3 drawPosition = transform.position + grapplePosition;
			Gizmos.DrawWireMesh(grappleMesh,drawPosition, Quaternion.Euler(grappleRotation));
		}
	}
	#endif
}
