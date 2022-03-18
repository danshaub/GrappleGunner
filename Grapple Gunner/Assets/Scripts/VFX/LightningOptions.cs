using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lightning Options", menuName = "Grapple Gunner/VFX/LightningOptions", order = 0)]

public class LightningOptions : ScriptableObject
{
    [Range(0f,1f)]
    public float rendererWidth = .1f;
    [Range(0f,1f)]
    public float unitsPerTile = 1f;
}