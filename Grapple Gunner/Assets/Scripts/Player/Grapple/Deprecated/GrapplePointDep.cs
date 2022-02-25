using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[ExecuteInEditMode]
public class GrapplePointDep : MonoBehaviour
{
	//Defines which kind of grapple interaction the grapple point will have.
	public enum GrappleType
	{
		Red = 0,
		Green = 1,
		Blue = 2,
		Orange = 3,
		OrangeDisabled = 4,
		Button = 5,
		None = 6
	}


	public GrappleType type;
	public bool useRaycastPosition = false;
	public Vector3 grapplePosition;
	public Vector3 grappleRotation;

	[Header("Options for Orange Point")]
	public Transform teleportParent;
	public Vector3 teleportOffset;
	public Material disabledMaterial;
	public bool infiniteUses = true;
	public int numberUses;
	private int remainingUses;


	[Header("Options for Button")]
	public UnityEvent onButtonPress;
	public float gizmoAlpha;

	[Header("Gizmo Mesh")]
	public Mesh grappleMesh; //= Resources.GetBuiltinResource<Mesh>("Cube.fbx");
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
		originalMaterial = GetComponent<MeshRenderer>().sharedMaterial;
		remainingUses = numberUses;

		if(!teleportParent){
			teleportParent = transform;
		}
	}

	public void DecrementUses(){
		if(infiniteUses){
			return;
		}
		else{
			remainingUses--;
			if(remainingUses == 0){
				type = GrappleType.OrangeDisabled;
				GetComponent<MeshRenderer>().sharedMaterial = disabledMaterial;
			}
		}
	}

	public void InvokeButtonEvent(){
		onButtonPress.Invoke();
	}



	// #if UNITY_EDITOR
	private float maxDimension(Vector3 v){
        
        return Mathf.Max(Mathf.Max(v.x, v.y), v.z);
        
	}

	private void DrawColliderOutline(){
        System.Type colliderType = GetComponent<Collider>().GetType();
        if (colliderType == typeof(MeshCollider))
        {
            MeshCollider buttonCollider = GetComponent<MeshCollider>();
            Gizmos.DrawWireMesh(buttonCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
        }
        else if (colliderType == typeof(BoxCollider))
        {
            BoxCollider buttonCollider = GetComponent<BoxCollider>();
			Gizmos.DrawWireCube(transform.TransformPoint(buttonCollider.center), 
								transform.TransformDirection(Vector3.Scale(buttonCollider.size, transform.lossyScale)));
        }
        else if (colliderType == typeof(SphereCollider))
        {
            SphereCollider buttonCollider = GetComponent<SphereCollider>();
            Gizmos.DrawWireSphere(transform.TransformPoint(buttonCollider.center),
								  buttonCollider.radius * maxDimension(transform.lossyScale));
        }
	}
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
				try{
					Gizmos.DrawCube((teleportParent.position + teleportOffset), teleportCube);
				} catch{
                    Gizmos.DrawCube((transform.position + teleportOffset), teleportCube);
				}
				break;
			case GrappleType.Button:
				Gizmos.color = Color.white;
                DrawColliderOutline();
				return;
		}
		if (useRaycastPosition){
			DrawColliderOutline();
		}
		else{
            Vector3 drawPosition = transform.position + grapplePosition;
			if(grappleMesh){
				Gizmos.DrawWireMesh(grappleMesh,drawPosition, Quaternion.Euler(grappleRotation));
			}
			else{
                Gizmos.DrawWireCube(drawPosition, teleportCube);
            }
		}
	}
	// #endif
}