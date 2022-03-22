using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerDep : MonoBehaviour
{
    public void LoadLevel(int levelIndex){
        // SceneManager.LoadScene(levelIndex);
    }

    public void ReloadCurrentLevel(){
        // LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit(){
        Application.Quit();
    }
}
