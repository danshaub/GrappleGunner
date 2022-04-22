using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionSystem : MonoBehaviour
{
    public ParticleSystem transitionParticles;
    public Animator fadeInOut;
    public bool useParticles;

    public void SetParticleColor(Color color)
    {
        transitionParticles.GetComponent<ParticleSystemRenderer>().trailMaterial.SetColor("_Color", color);
    }
    public void StartTransition()
    {
        fadeInOut.SetTrigger("FadeIn");
        if (useParticles)
        {
            transitionParticles.Play();
            SFXManager.Instance.PlaySFX("TransitionStart");
            SFXManager.Instance.FadeInSFX("TransitionSustain", .25f);
        }


        StartCoroutine(PauseTransition());
    }

    private IEnumerator PauseTransition()
    {
        yield return new WaitForSeconds(.25f);

        if (useParticles)
        {
            transitionParticles.Pause();
        }
        
    }

    public void EndTransition()
    {
        if (transitionParticles.IsAlive())
        {
            StartCoroutine(EndTransitionCoroutine());
        }
    }

    private IEnumerator EndTransitionCoroutine()
    {
        while (useParticles && !transitionParticles.isPaused)
        {
            yield return new WaitForEndOfFrame();
        }

        if (useParticles)
        {
            transitionParticles.Play();
        }

        SFXManager.Instance.FadeOutSFX("TransitionSustain", .25f, true);
        SFXManager.Instance.PlaySFX("TransitionEnd");

        fadeInOut.SetTrigger("FadeOut");
    }
}