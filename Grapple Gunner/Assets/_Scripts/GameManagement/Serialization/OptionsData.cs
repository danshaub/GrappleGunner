using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsData : ScriptableObject
{
    public bool snapTurn = true;
    public bool useSpeedLines = true;
    public int snapValue = 4;
    public float continuousTrunSpeed = 60f;
}