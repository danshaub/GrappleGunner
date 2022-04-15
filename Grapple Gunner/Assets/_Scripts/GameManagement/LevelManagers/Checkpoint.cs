using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour
{
    public Transform respawnPosition;
    public UnityEvent onCheckpointReached;
    public UnityEvent onCheckpointRespawn;
    private bool used = false;

    private void OnTriggerEnter(Collider other) {
        if(used) return;
        onCheckpointReached.Invoke();
        ((LevelManager)LevelManager.Instance).MakeCheckpoint(respawnPosition, onCheckpointRespawn);
        used = true;
    }

    private void OnDrawGizmos() {
        BoxCollider c = GetComponent<BoxCollider>();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.TransformPoint(c.center), transform.TransformVector(c.size));

        Gizmos.DrawLine(respawnPosition.position, transform.TransformPoint(c.center));
    }
}
