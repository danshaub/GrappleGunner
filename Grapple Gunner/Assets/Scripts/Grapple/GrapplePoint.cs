using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public GrappleType type;
    public bool useRaycastPosition = false;
    public Vector3 grapplePosition;


    //Defines which kind of grapple interaction the grapple point will have.
    public enum GrappleType{
        Red,
        Green,
        Blue,
        Orange
    }

    #if UNITY_EDITOR
        private void OnDrawGizmos() {
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
                Gizmos.DrawWireMesh(GetComponent<MeshFilter>().sharedMesh,transform.position, transform.rotation, transform.localScale);
            }
            else{
                Gizmos.DrawSphere(transform.position + grapplePosition, .1f);
            }
        }
    #endif
}
