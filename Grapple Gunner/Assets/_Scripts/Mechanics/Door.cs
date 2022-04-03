using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform doorTransform;
    public Transform openTransform;
    public Transform closedTransform;
    private Vector3 targetPosition;
    public float speed;
    public bool startClosed = true;
    // Start is called before the first frame update
    void Start()
    {
        if (startClosed)
        {
            doorTransform.localPosition = closedTransform.localPosition;
            targetPosition = closedTransform.localPosition;
        }
        else
        {
            doorTransform.localPosition = openTransform.localPosition;
            targetPosition = openTransform.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, targetPosition, speed * Time.deltaTime);
    }

    public void Open()
    {
        targetPosition = openTransform.localPosition;
    }

    public void Close()
    {
        targetPosition = closedTransform.localPosition;
    }

    private void OnDrawGizmos()
    {
        Mesh visualMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        Color o = new Color(1, .5f, 0);
        Color c = Color.cyan;
        Gizmos.color = o;
        Gizmos.DrawWireMesh(visualMesh, openTransform.position, openTransform.rotation, openTransform.lossyScale);
        Gizmos.color = c;
        Gizmos.DrawWireMesh(visualMesh, closedTransform.position, closedTransform.rotation, closedTransform.lossyScale);

    }
}
