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
    private bool released = false;


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
