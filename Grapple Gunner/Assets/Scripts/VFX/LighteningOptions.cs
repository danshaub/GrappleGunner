using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lightening Options", menuName = "Grapple Gunner/VFX/LighteningOptions", order = 0)]

public class LighteningOptions : ScriptableObject
{
    [Range(1, 10)]
    public int nubmerArks = 1;

    [Range(0.01f, 1f)]
    public float drawThreshold = 0.2f;

    [Range(0.01f, 2)]
    public float targetSegmentLength = 0.25f;

    [Range(0f, 1f)]
    public float positionRange = 0.15f;

    [Range(1, 4)]
    public int updateRate = 2;

    [Range(0f,1f)]
    public float rendererWidth = .1f;
}