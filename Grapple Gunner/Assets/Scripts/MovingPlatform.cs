using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    new public Rigidbody rigidbody;
    public Vector3[] points;
    public int pointNumber = 0;
    private Vector3 currentTarget;

    public float tolerance;
    public float speed;
    public float delayTime;

    private float delay_start;

    public bool automatic;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbody.useGravity = false;

        if(points.Length > 0){
            currentTarget = points[0];
        }

        tolerance = speed * Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position != currentTarget){
            MovePlatform();
        }
        else{
            UpdateTarget();
        }
    }

    private void MovePlatform(){
        Vector3 heading = currentTarget - transform.position;
        rigidbody.MovePosition(transform.position + heading.normalized * speed * Time.fixedDeltaTime); 
        if(heading.magnitude < tolerance){
            rigidbody.MovePosition(currentTarget);
            delay_start = Time.time;
        }
    }

    private void UpdateTarget(){
        if(automatic){
            if(Time.time - delay_start > delayTime){
                NextPlatform();
            }
        }
    }

    public void NextPlatform(){
        pointNumber = (pointNumber + 1) % points.Length;

        currentTarget = points[pointNumber];
    }
}
