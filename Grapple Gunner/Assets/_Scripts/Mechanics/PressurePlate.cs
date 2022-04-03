using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent onPressurePlatePress;
    public UnityEvent onPressurePlateRelease;

    public Transform plateTransform;
    public float springDistance;
    private bool released = true;
    private SpringJoint joint;

    private void Start() {
        joint = GetComponentInChildren<SpringJoint>();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(plateTransform.localPosition.y) > springDistance)
        {
            if (released)
            {
                onPressurePlatePress.Invoke();
            }
            released = false;
        }
        else
        {
            if (!released)
            {
                onPressurePlateRelease.Invoke();
            }
            released = true;
        }
    }
}
