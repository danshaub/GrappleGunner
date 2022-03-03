using UnityEngine;

public interface I_GrappleInteraction
{
    public void OnHit();
    public void OnRelease();
    public void OnFixedUpdate();
    public void OnReelIn(float reelStrength);
    public void OnReelOut();
    public void OnSwing(Vector3 swingVelocity);
}
