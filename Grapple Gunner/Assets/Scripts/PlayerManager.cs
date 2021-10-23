using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public InputActionReference jumpReference = null;

    // Start is called before the first frame update
    private void Awake()
    {
        jumpReference.action.started += Jump;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        jumpReference.action.started -= Jump;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("hello jump");
        transform.Translate(new Vector3(0,1,0));
    }
}
