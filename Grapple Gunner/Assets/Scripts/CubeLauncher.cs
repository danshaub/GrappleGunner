using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLauncher : MonoBehaviour
{
    public Rigidbody rbToLaunch;
    public Vector3 launchLocation;
    public Vector3 launchRotation;
    public Vector3 launchVelocity;
    public float launchPeriod;
    public bool repeatLaunch;

    private void Start() {
        if(repeatLaunch){
            InvokeRepeating("Launch", 0f, launchPeriod);
        }
    }

    public void Launch(){
        rbToLaunch.angularVelocity = Vector3.zero;

        rbToLaunch.position = launchLocation;
        rbToLaunch.rotation = Quaternion.Euler(launchRotation);
        rbToLaunch.velocity = launchVelocity;
    }

    private void OnDrawGizmos() {
        Mesh objMesh;
        Gizmos.color = Color.magenta;
        try{
            objMesh = rbToLaunch.gameObject.GetComponent<MeshFilter>().sharedMesh;
            Gizmos.DrawWireMesh(objMesh, launchLocation, Quaternion.Euler(launchRotation), rbToLaunch.transform.lossyScale);
        }catch{
            Gizmos.DrawWireCube(launchLocation, Vector3.one);
        }

        Gizmos.DrawRay(launchLocation, launchVelocity);
    }
}