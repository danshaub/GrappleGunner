using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaypointMover : MonoBehavior(){
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;
    private Transform currentWaypoint;
    void Start(){
        currentWayPoint = waypoints.GetNextWaypoint(currentWayPoint);
        transform.position = currentWayPoint.position;
        currentWayPoint = waypoints.GetNextWaypoint(currentWayPoint);
        transform.LookAt(currentWayPoint);
    }
    void Update(){
        transform.position = Vector3.MoveTowards(transform.position, currentWayPoint.position, moveSpeed * Time.deltatime);
        if (Vector3.Distance(transform.position, currentWayPoint.position) < distanceThreshold){
            currentWayPoint = waypoints.GetNextWaypoint(currentWayPoint);
            transform.LookAt(currentWayPoint);
        }
    }
}