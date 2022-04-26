using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrangeOptions", menuName = "Grapple Gunner/OrangeOptions", order = 4)]
public class OrangeOptions : ScriptableObject
{
    public float teleportTransitionTime = .25f;
    public float reelInThreshold;
    public Color particleColor = new Color(1f,0.4f,0f);

    public float destructionTime = 0.25f;
    public GameObject destructionPS;
    public GameObject respawnPS;

    [Header("BonkSFX")]
    public float bonkCooldown;
    public float bonkThreshold;
    public AnimationCurve bonkVolume;
    public AudioClip bonkSound;
    public AudioClip bonkBarrierSound;
}
