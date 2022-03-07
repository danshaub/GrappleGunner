using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBlock : MonoBehaviour
{
    public Transform deathTeleportPoint;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerManager.Instance.TeleportPlayer(deathTeleportPoint);
        }
    }
}
