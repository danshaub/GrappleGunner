using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVisual : MonoBehaviour
{
    CapsuleCollider c;
    Vector3 newScale = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        c = transform.parent.gameObject.GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = c.center;
        newScale.x = c.radius * 2;
        newScale.z = c.radius * 2;
        newScale.y = c.height * .5f;

        transform.localScale = newScale;
    }
}
