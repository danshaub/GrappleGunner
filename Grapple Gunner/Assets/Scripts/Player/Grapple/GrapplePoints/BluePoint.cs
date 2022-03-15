using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BluePoint : GrapplePoint
{
    public Vector3 defaultHookPosition;
    public Transform pointVisual;
    private Collider pointCollider;

    private Vector3 targetLocalPosition;
    private float targetLocalScale;
    private float lerpValue;
    private bool showingMiniPoint = false;
    override protected void Awake()
    {
        base.Awake();
        type = GrappleType.Blue;
        useRaycastPosition = true;

        pointCollider = GetComponent<Collider>();
    }
    override public void OnPointHit()
    {
        pointVisual.parent = transform;
        return;
    }

    override public void OnPointReleased()
    {
        return;
    }

    private void Update() {
        if(showingMiniPoint){
            pointVisual.localPosition = Vector3.Lerp(pointVisual.localPosition, targetLocalPosition, lerpValue);
            pointVisual.localScale = Vector3.Lerp(pointVisual.localScale, targetLocalScale * Vector3.one, lerpValue);
            pointVisual.rotation = Quaternion.Slerp(pointVisual.rotation, Random.rotation, lerpValue);
        }
        else{
            pointVisual.localPosition = Vector3.Lerp(pointVisual.localPosition, Vector3.zero, lerpValue);
            pointVisual.localScale = Vector3.Lerp(pointVisual.localScale, Vector3.one, lerpValue);
            pointVisual.localRotation = Quaternion.Slerp(pointVisual.localRotation, Quaternion.identity, lerpValue);
        }
    }

    public void ShowMiniPoint(Transform parent, Vector3 targetPosition, float targetScale, float interpolationValue){
        targetLocalPosition = targetPosition;
        targetLocalScale = targetScale;
        lerpValue = interpolationValue;

        showingMiniPoint = true;

        pointVisual.position = transform.position;
        pointVisual.rotation = transform.rotation;
        pointVisual.localScale = Vector3.one;

        pointVisual.parent = parent;

        pointCollider.enabled = false;

    }

    public void HideMiniPoint(){
        showingMiniPoint = false;
        pointVisual.parent = transform;

        pointCollider.enabled = true;
    }
}