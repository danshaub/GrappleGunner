using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public int LevelNum = 1;

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")){
            // SceneManager.LoadScene(LevelNum);
        }
    }

    public void Activate(){
        // SceneManager.LoadScene(LevelNum);
    }
}
