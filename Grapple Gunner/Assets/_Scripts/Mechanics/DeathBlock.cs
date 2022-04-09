using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBlock : MonoBehaviour
{
    private bool killedPlayer = false;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        if (killedPlayer) return;
        LevelManager.Instance.KillPlayer();

        killedPlayer = true;
        Invoke("ResetKilledPlayer", 1f);
    }

    private void ResetKilledPlayer()
    {
        killedPlayer = false;
    }
}
