using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnMenu : MonoBehaviour
{
    public void Respawn(){
        LevelManager.Instance.RespawnPlayer();
    }
    public void ReturnToMainMenu(){
        LevelManager.Instance.LoadMainMenu();
    }
}
