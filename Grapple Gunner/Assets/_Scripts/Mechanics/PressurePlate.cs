using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent onPressurePlatePress;
    public UnityEvent onPressurePlateRelease;

    private bool released = false;

}
