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
    [HideInInspector]
    public List<GameObject> TrainingQueue;
    public int TimeLeft;
    public bool Training;
    public bool isDropOffPoint;
    public GameObject GUIGhost;
    Game game;

    void Start()
    {
        isWalkable = false;
        ResearchableTechs = new List<Tech>();
        TrainingQueue = new List<GameObject>();
        RallyPoint = gameObject.transform.position;
        RallyTarget = null;
        game = GameObject.Find("Game").gameObject.GetComponent<Game>();

    }

    IEnumerator Train()
    {
        Training = true;
        //Az épület éppen kiképez
        GameObject TrainableUnit = TrainingQueue[0];
        Unit unitObj = TrainableUnit.GetComponent<Unit>();

        for (int TimeLeft = 0; TimeLeft < unitObj.trainingTime; TimeLeft++)
        {
            yield return new WaitForSeconds(1f);
        }

        GameObject TrainedUnit = Instantiate(TrainableUnit, transform.position, transform.rotation) as GameObject;
        TrainedUnit.GetComponent<Unit>().Owner = gameObject.GetComponent<Structure>().Owner;
        TrainedUnit.SetActive(true);
        TrainedUnit.transform.parent = gameObject.transform.parent;
        if (transform.parent.parent.gameObject.name != game.currentSolarSystem.Name)
        {

            TrainedUnit.GetComponent<Collider>().enabled = true;
            Renderer[] R = TrainedUnit.transform.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer rend in R)
            {
                rend.enabled = false;
            }

        }
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
        if (TrainedUnit.GetComponent<Unit>().isGatherer)
            Game.players.Where(x => x.empireName.Equals(Owner)).SingleOrDefault().CurrentWorkers.Add(TrainedUnit);

        TrainingQueue.RemoveAt(0);
        Training = false;
        if (TrainingQueue.Count != 0)
            StartCoroutine("Train");
        yield return null;
    }
}
