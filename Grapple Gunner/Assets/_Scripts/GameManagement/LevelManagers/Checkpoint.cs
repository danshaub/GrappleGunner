using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour
{
    public Transform respawnPosition;
    private bool used = false;

    private void OnTriggerEnter(Collider other) {
        if(used) return;
        ((LevelManager)LevelManager.Instance).MakeCheckpoint(respawnPosition);
        used = true;
    }

    private void OnDrawGizmos() {
        BoxCollider c = GetComponent<BoxCollider>();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.TransformPoint(c.center), transform.TransformVector(c.size));

        Gizmos.DrawLine(respawnPosition.position, transform.TransformPoint(c.center));
    }
}
