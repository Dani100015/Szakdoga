using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : Unit
{
    
    List<Tech> ResearchableTechs;
    Vector3 RallyPoint;
    Queue TrainingQueue;
    int TimeLeft;
    bool Training;

    void Start()
    {
        isWalkable = false;
        ResearchableTechs = new List<Tech>();
        TrainingQueue = new Queue();
    }

    void StartTrain(Unit unit)
    {
        Training = true;
        TimeLeft = unit.trainingTime;
    }

    void StartResearch(Tech tech)
    {
        Training = true;
        TimeLeft = tech.ResearchTime;
    }

    void EndTrain()
    {
       
    }

    void EndResearch()
    {

    }

    void Update()
    {
        if (Training)
        {
            if (TimeLeft != 0)
                TimeLeft -= Mathf.RoundToInt(Time.deltaTime);
            else
            {
                Training = false;
                if (TrainingQueue.Peek() is Unit)
                { }

            }
        }
    }
}
