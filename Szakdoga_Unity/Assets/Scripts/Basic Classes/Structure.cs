using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Structure : Unit
{

    public List<Tech> ResearchableTechs;
    public Vector3 RallyPoint;
    public Transform RallyTarget;
    Queue TrainingQueue;
    int TimeLeft;
    bool Training;
    public bool isDropOffPoint;

    void Start()
    {
        isWalkable = false;
        ResearchableTechs = new List<Tech>();
        TrainingQueue = new Queue();
        RallyPoint = gameObject.transform.position;
        RallyTarget = null;
    }

    IEnumerator Train(string unit)
    {
        Training = true;
        yield return new WaitForSeconds((Resources.Load("Prefabs/Units/"+unit) as GameObject).GetComponent<Unit>().trainingTime);
        GameObject TrainedUnit = Instantiate(Resources.Load(unit), transform.position, transform.rotation) as GameObject;
        TrainedUnit.GetComponent<Unit>().Owner = gameObject.GetComponent<Structure>().Owner;
        if (RallyPoint != gameObject.transform.position)
        {
            if (RallyTarget == null)
                TrainedUnit.GetComponent<AIDestinationSetter>().ai.destination = RallyPoint;
            else
                TrainedUnit.GetComponent<AIDestinationSetter>().target = RallyTarget;
        }
        TrainedUnit.transform.position = new Vector3(transform.position.x, transform.position.y ,transform.position.z - (TrainedUnit.GetComponent<Collider>().bounds.size.z + 10));
        yield return null;
    }

    IEnumerator Research(Tech tech)
    {
        Training = true;
        yield return new WaitForSeconds(tech.ResearchTime);
    }

    void Update()
    {
        //StartCoroutine("Train", "Gatherer");
    }
}
