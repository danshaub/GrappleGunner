using UnityEngine;

public interface I_GrappleInteraction
{
    public void OnHit();
    public void OnRelease();
    public void OnFixedUpdate();
    public void OnReelIn();
    public void OnReelOut();
    public void OnSwing();
}
