using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
    public Speedline speedlines;
    public TransitionSystem transitionSystem;
    public Color defaultTransitionColor;
    public Color deathTransitionColor;
}
