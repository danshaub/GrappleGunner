using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FieldVFX))]
[RequireComponent(typeof(Collider))]
public class Field : MonoBehaviour
{
    public bool isActive = true;
    private FieldVFX vfx;
    private Collider col;

    private void Start() {
        vfx = GetComponent<FieldVFX>();
        col = GetComponent<Collider>();
        if(!isActive) SetInactive();
    }

    public void ToggleActive()
    {
        if (isActive) SetInactive();
        else SetActive();
    }

    public void SetActive()
    {
        isActive = true;
        col.enabled = true;
        vfx.SetActive();
    }

    public void SetInactive()
    {
        isActive = false;
        col.enabled = false;
        vfx.SetInactive();
    }
}
