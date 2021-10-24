using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public float jumpAmount = 10;
    public InputActionReference jumpReference = null;

    private Rigidbody rigidbody = null;

    // Start is called before the first frame update
    private void Awake()
    {
        // subscribe to events
        jumpReference.action.started += Jump;

        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        jumpReference.action.started -= Jump;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        rigidbody.velocity = Vector3.up * jumpAmount;
    }
}
