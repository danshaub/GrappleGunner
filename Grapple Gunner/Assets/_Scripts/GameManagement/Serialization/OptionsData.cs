using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsData : ScriptableObject
{
    public bool snapTurn = true;
    public bool useSpeedLines = true;
    public int snapValue = 4;
    public float continuousTrunSpeed = 60f;
    public float sfxVolume = 0f;
    public float musicVolume = 0f;
    public float voiceVolume = 0f;
    public float ambientVolume = 0f;
}