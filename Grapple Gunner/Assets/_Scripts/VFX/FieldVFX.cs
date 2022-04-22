using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class FieldVFX : MonoBehaviour
{
    public ParticleSystem fog;
    public ParticleSystem lightning;
    public Transform posXplane;
    public Transform negXplane;
    public Transform posYplane;
    public Transform negYplane;

    private BoxCollider coll;

    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
    }

    public void SetActive(){
        isActive = true;
        lightning.Play();

        StartCoroutine(FadeInSound(.75f, 1f));
    }

    public void SetInactive(){
        isActive = false;
        lightning.Stop();

        StartCoroutine(FadeOutSound(.75f));
    }

    private IEnumerator FadeInSound(float decayTime, float targetVolume)
    {
        audioSource.Play();

        float decayRate = targetVolume / decayTime;

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += (decayRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator FadeOutSound(float decayTime)
    {
        float decayRate = audioSource.volume / decayTime;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= (decayRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        audioSource.Stop();
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

        posXplane.localPosition = Vector3.right * coll.size.x * .5f;
        negXplane.localPosition = Vector3.right * coll.size.x * -.5f;
        posYplane.localPosition = Vector3.up * coll.size.y * .5f;
        negYplane.localPosition = Vector3.up * coll.size.y * -.5f;

        Gizmos.color = new Color(color.r, color.g, color.b, 0.25f);

        Gizmos.DrawCube(transform.TransformPoint(coll.center), transform.TransformVector(coll.size));
    }
}
