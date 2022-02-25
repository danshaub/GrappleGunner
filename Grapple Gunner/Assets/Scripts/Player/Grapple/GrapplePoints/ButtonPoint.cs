using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonPoint : MonoBehaviour
{
    public GrapplePoint.GrappleType type = GrapplePoint.GrappleType.Button;
    public UnityEvent onButtonPress;

    void OnPointHit()
    {
        onButtonPress.Invoke();
    }

}
