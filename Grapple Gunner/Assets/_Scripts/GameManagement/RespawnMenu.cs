using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnMenu : MonoBehaviour
{
    public void Respawn(){
        LocationManager.Instance.RespawnPlayer();
    }
    public void ReturnToMainMenu(){
        LocationManager.Instance.LoadMainMenu();
    }
}
