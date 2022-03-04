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
        returnPosition = visualTransform.localPosition;
    }
    private void FixedUpdate()
    {
        visualTransform.localPosition = Vector3.Lerp(visualTransform.localPosition, returnPosition,returnSpeed*Time.fixedDeltaTime);
    }

    public void MoveButton(){
        visualTransform.localPosition = targetPosition;
    }
}
