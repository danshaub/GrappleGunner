using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform doorTransform;
    public Transform openTransform;
    public Transform closedTransform;
    public float speed;
    public bool closed = true;
    // Start is called before the first frame update
    void Start()
    {
        if (closed)
        {
            doorTransform.localPosition = closedTransform.localPosition;
        }
        else
        {
            doorTransform.localPosition = openTransform.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (closed)
        {
            doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, closedTransform.localPosition, speed * Time.deltaTime);
        }
        else
        {
            doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, openTransform.localPosition, speed * Time.deltaTime);
        }
    }

    public void Open()
    {
        closed = false;
    }

    public void Close()
    {
        closed = true;
    }

    public void Toggle(){
        closed = !closed;
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
