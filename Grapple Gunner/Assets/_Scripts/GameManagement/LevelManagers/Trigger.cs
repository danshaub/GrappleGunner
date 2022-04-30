using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Trigger : MonoBehaviour, ISaveState
{
    public UnityEvent onReached;
    private bool used = false;
    private bool usedAtCheckpoint = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        onReached.Invoke();
        used = true;
    }

    private void OnDrawGizmos()
    {
        BoxCollider c = GetComponent<BoxCollider>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.TransformPoint(c.center), transform.TransformVector(c.size));
    }

    public void SaveState()
    {
        usedAtCheckpoint = used;
    }

    public void LoadState()
    {
        used = usedAtCheckpoint;
    }
}
