using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    [SerializeField] private PlayerPhysics playerPhysics;
    [SerializeField] public bool allowMovement = true;
    [SerializeField] public bool grounded = true;
    public float playerHeight = 0;
    public Vector3 playerXZLocalPosistion;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }
    public void StopGrounded(){
        playerPhysics.StopGrounded();
    }
}
