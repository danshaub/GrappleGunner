using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 18) // Block
        {
            BluePointVisual visual = other.gameObject.GetComponent<BluePointVisual>();
            BluePoint point = other.gameObject.GetComponentInParent<BluePoint>();
            if (point != null)
            {
                if (point.blockHeld)
                {
                    point.queueDestroyBlock = true;
                }
                else
                {
                    point.DestroyBlock();
                }
            }
            else
            {
                visual.QueueDestroyBlock();
            }
        }

        for (int index = 0; index < 2; index++)
        {
            I_GrappleInteraction interaction = GrappleManager.Instance.grappleInteractions[index];
            if (interaction?.GetType() == typeof(BlueInteraction))
            {
                BlueInteraction blueInteraction = (BlueInteraction)interaction;
                if (blueInteraction.bluePoint.queueDestroyBlock)
                {
                    blueInteraction.DestroyBlock();
                }
            }
        }
    }
}