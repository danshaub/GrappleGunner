using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3) // Player
        {
            for (int index = 0; index < 2; index++)
            {
                I_GrappleInteraction interaction = GrappleManager.Instance.grappleInteractions[index];
                if (interaction?.GetType() == typeof(BlueInteraction))
                {
                    BlueInteraction blueInteraction = (BlueInteraction)interaction;
                    if (blueInteraction.blockIsStored)
                    {
                        blueInteraction.DestroyBlock();
                    }
                }
            }
        }
        else if (other.gameObject.layer == 13 || other.gameObject.layer == 14) // Block
        {
            BluePoint point = other.gameObject.GetComponent<BluePoint>();
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
