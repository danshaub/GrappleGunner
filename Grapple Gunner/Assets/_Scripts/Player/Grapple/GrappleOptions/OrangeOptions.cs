using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrangeOptions", menuName = "Grapple Gunner/OrangeOptions", order = 4)]
public class OrangeOptions : ScriptableObject
{
    public float teleportTransitionTime = .25f;
    public float reelInThreshold;
    public Color particleColor = new Color(1f,0.4f,0f);
}
