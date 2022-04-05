using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedline : MonoBehaviour
{
    private float speed;
    private ParticleSystem speedlines;
    private ParticleSystem.MainModule main;
    public bool active = true;
    public AnimationCurve particleAlphaSpeedCurve;
    private Rigidbody rb;

    void Start()
    {
        rb = PlayerManager.Instance.movementController.rigidbody;
        speedlines = GetComponent<ParticleSystem>();
        main = speedlines.main;
        main.startColor = Color.white;
    }

    void FixedUpdate()
    {
        if (!active) return;

        speed = rb.velocity.magnitude;
        float alpha = particleAlphaSpeedCurve.Evaluate(speed);

        if (alpha > float.Epsilon)
        {
            gameObject.transform.forward = rb.velocity.normalized;
            main.startColor = new Color(1, 1, 1, alpha);
            speedlines.Play();
        }
        else
        {
            speedlines.Stop();
        }

    }
}