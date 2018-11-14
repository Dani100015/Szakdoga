using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class Structure : Unit
{

    public List<Tech> ResearchableTechs;
    public List<GameObject> TrainableUnits;
    public Vector3 RallyPoint;
    public Transform RallyTarget;
    [HideInInspector]public List<object> TrainingQueue;
    public int TimeLeft;
    bool Training;
    public bool isDropOffPoint;
    public GameObject GUIGhost;

    void Start()
    {
        isWalkable = false;
        ResearchableTechs = new List<Tech>();
        TrainingQueue = new List<object>();
        RallyPoint = gameObject.transform.position;
        RallyTarget = null;
    }
  
    IEnumerator Train()
    {
        Debug.Log("indul");
        //Technológiát fejlesztünk?
        if (TrainingQueue[0] is Tech)
            yield return null;

        //Az épület éppen kiképez/fejleszt
        GameObject TrainableUnit = TrainingQueue[0] as GameObject;
        Unit unitObj = TrainableUnit.GetComponent<Unit>();

        for (int TimeLeft = 0; TimeLeft < unitObj.trainingTime; TimeLeft++)
        {
            yield return new WaitForSeconds(1f);
        }

        GameObject TrainedUnit = Instantiate(TrainableUnit, transform.position, transform.rotation) as GameObject;
        TrainedUnit.GetComponent<Unit>().Owner = gameObject.GetComponent<Structure>().Owner;
        AIDestinationSetter setter = TrainedUnit.GetComponent<AIDestinationSetter>();
        if (RallyPoint != gameObject.transform.position)
        {
            if (RallyTarget == null)
            {
                setter.ai.destination = RallyPoint;
                setter.ai.isStopped = false;
            }
            else
                setter.target = RallyTarget;
        }
        TrainedUnit.transform.position = new Vector3(transform.position.x, -2.1f, transform.position.z - (TrainedUnit.GetComponent<Collider>().bounds.size.z));
        TrainedUnit.name = TrainableUnit.name;
        //Kiképzett egység hozzáadása az egységek listához, fejlesztésekkel való módosításhoz
        Game.players.Where(x => x.empireName.Equals(Owner)).SingleOrDefault().units.Add(TrainedUnit);

        TrainingQueue.RemoveAt(0);
        if (TrainingQueue.Count != 0)
        {
            if (TrainingQueue[0] is Tech)
                StartCoroutine("Research");
            else StartCoroutine("Train");
        }
        Debug.Log(TrainingQueue.Count);
        yield return null;
    }
}
