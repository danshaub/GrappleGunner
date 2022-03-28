using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")){
            Activate();
        }
    }

    public void Activate(){
        LevelManager.Instance.LoadNextLevel();
    }
}
