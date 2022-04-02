using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent onPressurePlatePress;
    public UnityEvent onPressurePlateRelease;

    public Vector3 anchorPosition;
    public Transform plateTransform;
    public float springDistance;
    private bool released = false;

    public void onPlatePress(){
        if (anchorPosition.y - plateTransform.localPosition.y <= .25f + springDistance){
            return;
        } else {
            released = false;
            onPressurePlatePress.Invoke();
        }
    }

    public void onPlateRelease(){
        if(released == true) return;

        released = true;
        onPressurePlateRelease.Invoke();
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color c = Color.green;
        c.a = .25f;
        Gizmos.color = c;
        Gizmos.DrawCube(anchorPosition, Vector3.one * .05f);
    }
#endif
}
