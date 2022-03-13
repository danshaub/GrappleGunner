using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Lightning : MonoBehaviour
{
    public Transform hookRopePointTransform;
    private LineRenderer lineRenderer;
    private int updateCounter = 0;
    private Vector3 endPoint;
    private int numberSegments;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        SetColor(GrappleManager.Instance.LightningColors.standardColor);
        lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, GrappleManager.Instance.LightningOptions.rendererWidth);
        lineRenderer.positionCount = 2;
    }

    private void OnEnable()
    {
        try
        {
            SetColor(GrappleManager.Instance.LightningColors.standardColor);
            lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, GrappleManager.Instance.LightningOptions.rendererWidth);
            lineRenderer.positionCount = 2;
        }
        catch { }

    }

    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hookRopePointTransform.position);

        float distance = Vector3.Distance(transform.position, hookRopePointTransform.position);
        lineRenderer.material.SetFloat("_Tiling", GrappleManager.Instance.LightningOptions.unitsPerTile * distance);
    }

    public void SetColor(Color newColor)
    {
        lineRenderer.material.SetColor("_Color", newColor);
    }
}
