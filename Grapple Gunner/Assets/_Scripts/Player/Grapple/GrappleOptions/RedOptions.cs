using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RedOptions", menuName = "Grapple Gunner/RedOptions", order = 2)]
public class RedOptions : ScriptableObject
{
    public float grappleSpeed;
    public float groundKick;
    public float velocityDamper;
    public float brakeDamper;
    public float speedIncreaseMultiplier;
    public AnimationCurve velocityCurve;
}
