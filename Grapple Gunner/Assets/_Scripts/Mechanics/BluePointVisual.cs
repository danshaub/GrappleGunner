using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePointVisual : MonoBehaviour
{
    private BluePoint point;

    private void Start() {
        point = GetComponentInParent<BluePoint>();
    }

    public void QueueDestroyBlock(){
        point.queueDestroyBlock = true;
    }
}
