using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class AStarPath : MonoBehaviour {

    #region Variables
    public Vector3 targetPosition;
    private Seeker seeker;
    private CharacterController controller;
    public Path path;

    public float speed;

    //The max distance from the AI to a Waypoint for it to continue to the next one
    [SerializeField]
    public float nextWaypointDistance = 10f;

    //Current waypoint (always starts at index 0)
    private int CurrentWaypoint = 0;
    #endregion

    public void Start()
    {
        targetPosition = GameObject.Find("Target").transform.position;
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();

        //Set Path
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            //Reset waypoint counter
            CurrentWaypoint = 0;
        }
    }

    public void FixedUpdate()
    {
        if (path == null)
            return;
        if (CurrentWaypoint >= path.vectorPath.Count)
            return;

        //Calculate direction of unit
        Vector3 dir = (path.vectorPath[CurrentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        controller.SimpleMove(dir); //Unit moves here

        //Check if close enough to the current waypoint, if we are, proceed to next waypoint
        if (Vector3.Distance(transform.position,path.vectorPath[CurrentWaypoint]) <= nextWaypointDistance)
        {
            CurrentWaypoint++;
            return;
        }
    }
   
}
