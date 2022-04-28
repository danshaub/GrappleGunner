using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent onPressurePlatePress;
    public UnityEvent onPressurePlateRelease;

    public AudioClip onPressSound;
    public AudioClip onReleaseSound;

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
                GetComponent<AudioSource>().PlayOneShot(onPressSound);
                onPressurePlatePress.Invoke();
            }
            released = false;
        }
        else
        {
            if (!released)
            {
                GetComponent<AudioSource>().PlayOneShot(onReleaseSound);
                onPressurePlateRelease.Invoke();
            }
            released = true;
        }

        if (Mathf.Abs(plateTransform.localPosition.y) > springDistance * 2){
            float newY = -(springDistance + springDistance * .05f);
            plateTransform.localPosition = Vector3.up * newY;
        }
    }
}
