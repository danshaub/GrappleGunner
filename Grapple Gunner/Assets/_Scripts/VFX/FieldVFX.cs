using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
public class FieldVFX : MonoBehaviour
{
    public ParticleSystem fog;
    public ParticleSystem lightning;
    private BoxCollider coll;

    public float fogVolume;
    public float lightningFrequency;
    public Color color;

    private bool isActive = true;

    
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider>();

        ParticleSystem.ShapeModule shape = fog.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = coll.size;
        shape.position = coll.center;

        ParticleSystem.EmissionModule emission = fog.emission;
        emission.rateOverTime = fogVolume * coll.size.x * coll.size.y * coll.size.z;
        ParticleSystem.MainModule main = fog.main;
        main.startColor = color;

        shape = lightning.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = coll.size;
        shape.position = coll.center;

        emission = lightning.emission;
        emission.rateOverTime = lightningFrequency * coll.size.x * coll.size.y * coll.size.z;

        if (Application.isEditor)
        {
            lightning.GetComponent<ParticleSystemRenderer>().sharedMaterials[1].SetColor("_Color", color);
        }
        else
        {
            lightning.GetComponent<ParticleSystemRenderer>().materials[1].SetColor("_Color", color);
        }

        if(!isActive) SetInactive();
    }

    public void SetActive(){
        isActive = true;

        lightning.Play();
    }

    public void SetInactive(){
        isActive = false;

        lightning.Stop();
    }

    private void OnDrawGizmos()
    {
        coll = GetComponent<BoxCollider>();

        ParticleSystem.ShapeModule shape = fog.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = coll.size;
        shape.position = coll.center;

        ParticleSystem.EmissionModule emission = fog.emission;
        emission.rateOverTime = fogVolume * coll.size.x * coll.size.y * coll.size.z;
        ParticleSystem.MainModule main = fog.main;
        main.startColor = color;

        shape = lightning.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = coll.size;
        shape.position = coll.center;

        emission = lightning.emission;
        emission.rateOverTime = lightningFrequency * coll.size.x * coll.size.y * coll.size.z;

        Gizmos.color = new Color(color.r, color.g, color.b, 0.25f);

        Gizmos.DrawCube(transform.TransformPoint(coll.center), transform.TransformVector(coll.size));
    }
}
