using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		UI,
		None
	}


	public GrappleType type;
	public bool useRaycastPosition = false;
	public Vector3 grapplePosition;

	public Vector3 grappleRotation;

	public Mesh grappleMesh;

	public Vector3 getGrapplePosition(){
		return transform.position + grapplePosition;
	}
	public Quaternion getGrappleRotation(){
		return Quaternion.Euler(grappleRotation);
	}

	private void Start() {
		gameObject.tag = "Hookable";
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
				Gizmos.color = Color.blue;
				break;
			case GrappleType.Orange:
				Gizmos.color = Color.yellow;
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
