using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class UnitPath : MonoBehaviour {

    #region Variables
    private Seeker seeker;
    private CharacterController controller;
    public Path path;
    private Unit unit;

    public float speed;
    public float nextWaypointDistance = 10;
    private int CurrentWaypoint = 0;
    #endregion

    public void Start()
    {      
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        unit = GetComponent<Unit>();
    }

    void LateUpdate()
    {
        if (unit.Selected && unit.isWalkable)
        {
            if (Input.GetMouseButtonDown(1))
            {               
                //Debug.Log(GameObject.Find("World").GetComponent<Mouse>().RightClickPoint);
                seeker.StartPath(transform.position, GameObject.Find("World").GetComponent<Mouse>().RightClickPoint, OnPathComplete);
            }
        }
    }

    //Pathfinding logic
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
        if (!unit.isWalkable)
            return;
        if (path == null)
            return;
        if (CurrentWaypoint >= path.vectorPath.Count)
            return;

        //Calculate direction of unit
        Vector3 dir = (path.vectorPath[CurrentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        controller.SimpleMove(dir); //Unit moves here

        //Check if close enough to the current waypoint, if we are, proceed to next waypoint
        if (Vector3.Distance(transform.position, path.vectorPath[CurrentWaypoint]) <= nextWaypointDistance)
        {
            CurrentWaypoint++;
            return;
        }
    }
}
