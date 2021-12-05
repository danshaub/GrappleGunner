using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePlayerMovement : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")){
            PlayerManager._instance.allowMovement = false;
        }
    }
    private void OnCollisionExit(Collision other) {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerManager._instance.allowMovement = true;
        }
    }
}
