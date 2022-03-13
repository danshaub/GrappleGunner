using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Lightening : MonoBehaviour
{
    public Transform hookRopePointTransform;
    private LineRenderer lineRenderer;
    private int updateCounter = 0;
    private Vector3 endPoint;
    private int numberSegments;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        try
        {
            SetColor(GrappleManager.Instance.LighteningColors.standardColor);
            lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, GrappleManager.Instance.LighteningOptions.rendererWidth);
        }
        catch{}

    }

    private void Update()
    {
        if (updateCounter == 0)
        {
            endPoint = hookRopePointTransform.position;

            float distance = Vector3.Distance(transform.position, endPoint);
            if (distance <= GrappleManager.Instance.LighteningOptions.targetSegmentLength)
            {
                numberSegments = 1;
                lineRenderer.positionCount = numberSegments + 1;

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, endPoint);

                // Update on every physics frame while close
                return;
            }

            numberSegments = (int)(distance / GrappleManager.Instance.LighteningOptions.targetSegmentLength) * GrappleManager.Instance.LighteningOptions.nubmerArks;

            int segmentsPerArk = (numberSegments) / GrappleManager.Instance.LighteningOptions.nubmerArks;
            lineRenderer.positionCount = numberSegments + 1;

            Vector3 lineDirection = (endPoint - transform.position).normalized;

            bool fromOrigin = false;
            for (int i = 0; i <= numberSegments; i++)
            {
                if (i % segmentsPerArk == 0)
                {
                    fromOrigin = !fromOrigin;
                }

                Vector3 position;
                if (fromOrigin)
                {
                    position = transform.position + (lineDirection * (distance / segmentsPerArk) * (i % segmentsPerArk));
                }
                else
                {
                    position = transform.position + (lineDirection * (distance / segmentsPerArk) * (segmentsPerArk - (i % segmentsPerArk)));
                }

                if (i % segmentsPerArk != 0)
                {
                    Vector3 offset = Random.insideUnitSphere * GrappleManager.Instance.LighteningOptions.positionRange;
                    offset = Vector3.ProjectOnPlane(offset, lineDirection);
                    position += offset;
                }

                lineRenderer.SetPosition(i, position);
            }
        }

        updateCounter = (updateCounter + 1) % GrappleManager.Instance.LighteningOptions.updateRate;
    }

    public void SetEndPoint(Vector3 newEndPoint)
    {
        endPoint = newEndPoint;
    }

    public void SetColor(Color newColor)
    {
        Gradient gradient = new Gradient();
        GradientColorKey colorKey = new GradientColorKey();
        GradientAlphaKey alphaKey = new GradientAlphaKey();

        colorKey.color = newColor;
        alphaKey.alpha = 1f;

        gradient.SetKeys(new GradientColorKey[] { colorKey }, new GradientAlphaKey[] { alphaKey });

        lineRenderer.colorGradient = gradient;
    }
}
