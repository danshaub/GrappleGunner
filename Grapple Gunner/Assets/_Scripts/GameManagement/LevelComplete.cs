using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    private bool used = false;
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")){
            Activate();
        }
    }

    public void Activate(){
        if(used) return;
        LocationManager.Instance.LoadNextLevel();
        used = true;
    }
}
