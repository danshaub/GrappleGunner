using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGun : MonoBehaviour
{

    [SerializeField] private GameObject reticleVisual;
    private Material reticleMaterial;
    [SerializeField] private Transform gunTip;

    public Transform debugRet;

    private void Start()
    {
        reticleMaterial = reticleVisual.GetComponent<Renderer>().material;
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
        if (Physics.SphereCast(gunTip.position, GrappleManager._instance.options.sphereCastRadius,
                              gunTip.forward, out hit, 2000, GrappleManager._instance.options.sphereCastMask))
        {
            hitMenu = hit.transform.gameObject.layer == 11;

            if (hit.transform.CompareTag("Hookable"))
            {
                GrapplePoint.GrappleType type = hit.transform.gameObject.GetComponent<GrapplePoint>().type;
                reticleMaterial.SetFloat("_Transparency", 1f);
                switch (type)
                {
                    case GrapplePoint.GrappleType.Red:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.red);
                        break;
                    case GrapplePoint.GrappleType.Green:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.green);
                        break;
                    case GrapplePoint.GrappleType.Blue:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.blue);
                        break;
                    case GrapplePoint.GrappleType.Orange:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.orange);
                        break;
                    case GrapplePoint.GrappleType.OrangeDisabled:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.disabled);
                        break;
                    case GrapplePoint.GrappleType.Button:
                        reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.button);
                        break;
                }
            }
            else
            {
                reticleMaterial.SetFloat("_Transparency", GrappleManager._instance.options.disabledTransparency);
                reticleMaterial.SetTexture("_MainTex", GrappleManager._instance.options.reticleManager.disabled);
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
            reticleDistance = Mathf.Clamp(distanceFromPoint, GrappleManager._instance.options.minReticleDistance, GrappleManager._instance.options.maxReticleDistance);
        }

        float reticleScale = GrappleManager._instance.options.reticleScaleCurve.Evaluate((reticleDistance / GrappleManager._instance.options.maxReticleDistance));
        reticleVisual.transform.localPosition = Vector3.forward * (reticleDistance + GrappleManager._instance.options.sphereCastRadius);
        reticleVisual.transform.localScale = new Vector3(reticleScale, reticleScale, 1);
    }
    #endregion
}
