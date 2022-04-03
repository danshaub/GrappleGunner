using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGun : MonoBehaviour
{

    [SerializeField] public GameObject reticleVisual;
    private Material reticleMaterial;
    [SerializeField] public Transform gunTip;
    [SerializeField] public Transform hookPoint;
    public Lightning lightning;
    private bool fired;

    private void Awake()
    {
        reticleMaterial = reticleVisual.GetComponent<Renderer>().material;
    }

    public void DrawRope()
    {
        if (hookPoint.gameObject.activeInHierarchy)
        {
            lightning.gameObject.SetActive(true);
        }
        else
        {
            lightning.gameObject.SetActive(false);
        }
    }


    #region Reticle Functions
    public void DisableReticle()
    {
        reticleVisual.SetActive(false);
    }
    public void EnableReticle()
    {
        reticleVisual.SetActive(true);
    }
    public void UpdateReticle()
    {
        if (!reticleVisual.activeInHierarchy)
        {
            return;
        }

        bool hitMenu = false;

        RaycastHit hit;
        if (Physics.SphereCast(gunTip.position, GrappleManager.Instance.options.sphereCastRadius,
                              gunTip.forward, out hit, 2000, GrappleManager.Instance.options.sphereCastMask))
        {
            hitMenu = hit.transform.gameObject.layer == 11;

            try
            {

                GrapplePoint.GrappleType type = hit.transform.gameObject.GetComponent<GrapplePoint>().type;
                reticleMaterial.SetFloat("_Transparency", 1f);
                switch (type)
                {
                    case GrapplePoint.GrappleType.Red:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager.Instance.options.reticleManager.red);
                        break;
                    case GrapplePoint.GrappleType.Green:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager.Instance.options.reticleManager.green);
                        break;
                    case GrapplePoint.GrappleType.Blue:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager.Instance.options.reticleManager.blue);
                        break;
                    case GrapplePoint.GrappleType.Orange:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager.Instance.options.reticleManager.orange);
                        break;
                    case GrapplePoint.GrappleType.OrangeDisabled:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager.Instance.options.reticleManager.disabled);
                        break;
                    case GrapplePoint.GrappleType.Button:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager.Instance.options.reticleManager.button);
                        break;
                }
            }
            catch
            {
                reticleMaterial.SetFloat("_Transparency", GrappleManager.Instance.options.disabledTransparency);
                reticleMaterial.SetTexture("_MainTex", GrappleManager.Instance.options.reticleManager.disabled);
            }
        }

        float distanceFromPoint = Vector3.Distance(gunTip.position, hit.point);


        float reticleDistance;

        if (hitMenu)
        {
            reticleDistance = distanceFromPoint;
        }
        else
        {
            reticleDistance = Mathf.Clamp(distanceFromPoint, GrappleManager.Instance.options.minReticleDistance, GrappleManager.Instance.options.maxReticleDistance);
        }

        float reticleScale = GrappleManager.Instance.options.reticleScaleCurve.Evaluate((reticleDistance / GrappleManager.Instance.options.maxReticleDistance));
        reticleVisual.transform.localPosition = Vector3.forward * (reticleDistance + GrappleManager.Instance.options.sphereCastRadius);
        reticleVisual.transform.localScale = new Vector3(reticleScale, reticleScale, 1);
    }
    #endregion
}
