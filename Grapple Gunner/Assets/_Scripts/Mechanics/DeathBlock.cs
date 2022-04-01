using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBlock : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    public void KillPlayer(){
        LevelManager.Instance.KillPlayer();
    }
}
