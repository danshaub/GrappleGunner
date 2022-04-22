using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ISaveState
{
    public Transform doorTransform;
    public Transform openTransform;
    public Transform closedTransform;

    public float speed;
    public bool closed = true;

    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    private State state;
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
        GetComponent<AudioSource>().PlayOneShot(doorOpenSound);
        closed = false;
    }

    public void Close()
    {
        GetComponent<AudioSource>().PlayOneShot(doorCloseSound);
        closed = true;
    }

    public void Toggle()
    {
        if (closed)
        {
            GetComponent<AudioSource>().PlayOneShot(doorOpenSound);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(doorCloseSound);
        }

        closed = !closed;
    }

    public void SaveState()
    {
        state = new State();
        state.closed = closed;
    }

    public void LoadState()
    {
        closed = state.closed;
        if (state.closed)
        {
            doorTransform.localPosition = closedTransform.localPosition;
        }
        else
        {
            doorTransform.localPosition = openTransform.localPosition;
        }
    }

    private struct State
    {
        public bool closed;
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
