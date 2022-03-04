using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonAnimation : MonoBehaviour
{
    public Transform visualTransform;
    public Vector3 targetPosition;
    public float returnSpeed;
    private Vector3 returnPosition;
    // Start is called before the first frame update
        void Start()
    {
        Invoke("MoveButton",5);
        returnPosition = visualTransform.localPosition;
    }
    private void FixedUpdate()
    {
        visualTransform.localPosition = Vector3.Lerp(visualTransform.localPosition, returnPosition,returnSpeed);
    }

    public void MoveButton(){
        visualTransform.localPosition = targetPosition;
    }
}
