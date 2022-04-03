using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPosition;
    private bool used = false;

    private void OnTriggerEnter(Collider other) {
        if(used) return;
        ((LevelManager)LevelManager.Instance).MakeCheckpoint(respawnPosition);
        used = true;
    }
}
