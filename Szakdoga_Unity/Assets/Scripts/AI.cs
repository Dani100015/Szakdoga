using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;

public class AI : MonoBehaviour {

    //Inspector adatok
    public int maxWorkerCount;
    public int defenceIntensity;
    public int attackIntensity;
    public bool CurrentlyAttacking;
    public bool AttackOnCooldown;

    public Player player;
    public ArrayList CurrentlySelectedUnits;
    public List<GameObject> PalladiumGatherers;
    public List<GameObject> IridiumGatherers;
    public List<GameObject> EezoGatherers;

    public List<GameObject> AttackForce;
    public List<GameObject> DefenceForce;
    public GameObject MainBuilding;
    public Structure mainStructure;
    public Structure mainBarracks;

    // Use this for initialization
    void Start () {
        player = Game.player2;
        CurrentlySelectedUnits = new ArrayList();
        PalladiumGatherers = new List<GameObject>();
        IridiumGatherers = new List<GameObject>();
        EezoGatherers = new List<GameObject>();
        AttackForce = new List<GameObject>();
        DefenceForce = new List<GameObject>();
        CurrentlyAttacking = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (player == null)
            player = Game.player2;
        
        if (MainBuilding == null)
        {
            player.units.Remove(player.units.Where(x => x.gameObject.Equals(MainBuilding)).FirstOrDefault());
            if (player.units.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().isDropOffPoint).Count() != 0)
            {
                MainBuilding = player.units.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().isDropOffPoint).FirstOrDefault();
                mainStructure = MainBuilding.GetComponent<Structure>();
            }

            if (MainBuilding == null)
            {
                if (player.CurrentWorkers.Count > 0 && BuildCostCheck(player.BuildableUnits.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().isDropOffPoint).FirstOrDefault().GetComponent<Structure>()))
                {
                    RebuildStructure(player.BuildableUnits.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().isDropOffPoint).First());
                }
                //Ide jön a vereség kondíció

            }
        }

        player.units.RemoveAll(x => x == null);


        #region Gyűjtögetők
        //Kiképzés
        if (mainStructure != null)
        {
            if (player.CurrentWorkers.Count + mainStructure.TrainingQueue.Where(x => x.GetComponent<Unit>().isGatherer).Count() < maxWorkerCount && mainStructure.TrainingQueue.Count < 7)
            {
                if (CostCheck(mainStructure.TrainableUnits.Where(x => x.GetComponent<Unit>() && x.GetComponent<Unit>().isGatherer).First().GetComponent<Unit>()))
                {
                    mainStructure.TrainingQueue.Add(mainStructure.TrainableUnits.Where(x => x.GetComponent<Unit>() && x.GetComponent<Unit>().isGatherer).First());
                    if (!mainStructure.Training)
                        mainStructure.StartCoroutine("Train");
                }
            }
        }

        //Irány gyűjteni
        if (player.IdleWorkers.Count != 0)
            SendToGather();
        #endregion

        #region Támadás/Védekezés
        //Védekezés
        DefenceForce.RemoveAll(x => x == null);

        if (mainBarracks == null)
        {
            mainBarracks = player.units.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().TrainableUnits.Count > 1).First().GetComponent<Structure>();
            if (mainBarracks == null)
            {
                if (player.CurrentWorkers.Count > 0 && BuildCostCheck(player.BuildableUnits.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().TrainableUnits.Count > 1).First().GetComponent<Structure>()))
                {
                    RebuildStructure(player.BuildableUnits.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().TrainableUnits.Count > 1).First());
                }
            }
        }
        if (mainBarracks != null && DefenceForce.Count < 10)
        {
            Unit currentlyTrainedUnit = mainBarracks.TrainableUnits[(int)Random.Range(0, mainBarracks.TrainableUnits.Count)].GetComponent<Unit>();
            if (CostCheck(currentlyTrainedUnit) && mainBarracks.TrainingQueue.Count < 7)
            {
                mainBarracks.TrainingQueue.Add(currentlyTrainedUnit.gameObject);
                if (!mainBarracks.Training)
                    mainBarracks.StartCoroutine("Train");
                DefenceForce.Add(player.units.Where(x => !DefenceForce.Contains(x) && !AttackForce.Contains(x) && x.GetComponent<Unit>() && !x.GetComponent<Unit>().isGatherer).First());

                Vector3 randomPos = Random.insideUnitSphere * 30;
                randomPos.y = 0;

                if (DefenceForce.Count != 0)
                {
                    if (DefenceForce.Last().GetComponent<AIDestinationSetter>())
                    {
                        DefenceForce.Last().GetComponent<AIDestinationSetter>().ai.destination = MainBuilding.transform.position + randomPos;
                        DefenceForce.Last().GetComponent<AIDestinationSetter>().ai.isStopped = false;
                    }
                }
            }
        }

        //Támadás
        AttackForce.RemoveAll(x => x == null);
        if (mainBarracks != null && AttackForce.Count < 8 && DefenceForce.Count == 10 && !CurrentlyAttacking)
        {
            Unit currentlyTrainedUnit = mainBarracks.TrainableUnits[(int)Random.Range(0, mainBarracks.TrainableUnits.Count)].GetComponent<Unit>();
            if (CostCheck(currentlyTrainedUnit) && mainBarracks.TrainingQueue.Count < 7)
            {
                mainBarracks.TrainingQueue.Add(currentlyTrainedUnit.gameObject);
                if (!mainBarracks.Training)
                    mainBarracks.StartCoroutine("Train");
                AttackForce.Add(player.units.Where(x => !DefenceForce.Contains(x) && !AttackForce.Contains(x) && x.GetComponent<Unit>() && !x.GetComponent<Unit>().isGatherer).First());

                Vector3 randomPos = Random.insideUnitSphere * 15;
                randomPos.y = 0;

                if (AttackForce.Count != 0)
                {
                    if (AttackForce.Last().GetComponent<AIDestinationSetter>())
                        AttackForce.Last().GetComponent<AIDestinationSetter>().ai.destination = mainBarracks.transform.position + randomPos;
                }
            }
        }

        //Ha megvan a támadó ereje, akkor támadást indít
        if (AttackForce.Count == 8 && !AttackOnCooldown && !CurrentlyAttacking)
        {
            StartCoroutine("AttackCooldown");
        }

        if (CurrentlyAttacking && AttackForce.Count == 0)
            CurrentlyAttacking = false;
        #endregion
    }

    void RebuildStructure(GameObject buildable)
    {
        GameObject builder = player.CurrentWorkers[(int)Random.Range(0, player.CurrentWorkers.Count - 1)] as GameObject;
        builder.GetComponent<Unit>().CurrentlyBuiltObject = buildable;
        builder.GetComponent<AIDestinationSetter>().ai.destination = new Vector3(Random.Range(-120,120), 0f, Random.Range(-100,100));
        builder.GetComponent<AIDestinationSetter>().ai.isStopped = false;
        builder.GetComponent<Unit>().StartCoroutine("Build");
    }

    IEnumerator AttackCooldown()
    {
        AttackOnCooldown = true;
        int i = 0;
        while (i < 20)
        {
            i++;
            yield return new WaitForSeconds(1f);
        }

        CurrentlyAttacking = true;
        Transform destination = player.enemies[(int)Random.Range(0, player.enemies.Count)].units.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().isDropOffPoint).First().transform;

        foreach (GameObject Attacker in AttackForce)
        {
            Transform RelayObject = GameObject.Find("SolarSystems").transform.Find(mainBarracks.transform.parent.parent.name).transform.Find("BUILDING_Relay");

            if (Attacker.transform.parent.parent != destination.parent.parent)
            {
                Unit unitObj = Attacker.GetComponent<Unit>();
                AIDestinationSetter setter = Attacker.GetComponent<AIDestinationSetter>();

                unitObj.solarSystemTarget = destination.parent.parent.gameObject;
                setter.target = RelayObject;
                yield return null;
            }
        }

        while(AttackForce.All(x =>x.transform.parent.parent != destination.parent.parent))
        {
            yield return null;
        }

        foreach (GameObject Attacker in AttackForce)
        {          
            Vector3 randomPos = Random.insideUnitSphere * 50;
            randomPos.y = 0;
            if (destination != null)
            {
                Attacker.GetComponent<AIDestinationSetter>().ai.destination = destination.position + randomPos;
                Attacker.GetComponent<AIDestinationSetter>().ai.isStopped = false;
            }
            else
            {
                CurrentlyAttacking = false;
                break;
            }
            yield return null;
        }
        AttackOnCooldown = false;
        yield return null;
    }

    bool CostCheck(Unit unit)
    {
        if (player.iridium - unit.iridiumCost >= 0 &&
            player.palladium - unit.palladiumCost >= 0 &&
            player.nullElement - unit.eezoCost >= 0 && player.CurrentPopulation + unit.PopulationCost <= player.MaxPopulation)
        {
            player.iridium -= unit.iridiumCost;
            player.palladium -= unit.palladiumCost;
            player.nullElement -= unit.eezoCost;
            player.CurrentPopulation += unit.PopulationCost;
            return true;
        }
        return false;
    }

    bool BuildCostCheck(Structure building)
    {
        if (player.iridium - building.iridiumCost >= 0 &&
            player.palladium - building.palladiumCost >= 0 &&
            player.nullElement - building.eezoCost >= 0)
            return true;
        return false;
    }

    public GameObject FindClosestResource(string type)
    {
        List<GameObject> gos = new List<GameObject>();
        //Itt kell átírni, hogyha csak a naprendszeren belül akar keresni
        foreach (Transform t in MainBuilding.transform.parent.parent.Find("Planets"))
        {
            if (t.tag.Equals(type))
                gos.Add(t.gameObject);
        }
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    void SendToGather()
    {
        //Ha elpusztult az egység, akkor kiveszi a listából
        IridiumGatherers.RemoveAll(x => x == null);
        PalladiumGatherers.RemoveAll(x => x == null);
        EezoGatherers.RemoveAll(x => x == null);

        GameObject unitObj = player.IdleWorkers[0];
        if (unitObj.GetComponent<Unit>().CurrentResourceAmount != 0)
            return;
        if (IridiumGatherers.Count < 5)
        {
            if (FindClosestResource("Iridium") != null)
            {
                unitObj.GetComponent<AIDestinationSetter>().target = FindClosestResource("Iridium").transform;
                IridiumGatherers.Add(unitObj);
                unitObj.GetComponent<Unit>().StartCoroutine("GatherTarget", unitObj.GetComponent<AIDestinationSetter>().target);
            }
            return;
        }
        if (PalladiumGatherers.Count < 5)
        {
            if (FindClosestResource("Palladium") != null)
            {
                unitObj.GetComponent<AIDestinationSetter>().target = FindClosestResource("Palladium").transform;
                PalladiumGatherers.Add(unitObj);
                unitObj.GetComponent<Unit>().StartCoroutine("GatherTarget", unitObj.GetComponent<AIDestinationSetter>().target);
            }
            return;
        }
        if (EezoGatherers.Count < 5)
        {
            if (FindClosestResource("ElementZero") != null)
            {
                unitObj.GetComponent<AIDestinationSetter>().target = FindClosestResource("ElementZero").transform;
                EezoGatherers.Add(unitObj);
                unitObj.GetComponent<Unit>().StartCoroutine("GatherTarget", unitObj.GetComponent<AIDestinationSetter>().target);
            }
            return;
        }

        //Ha épp idle az egység valamiért, akkor kivesszük a listából, majd újraosztjuk
        if (IridiumGatherers.Contains(unitObj))
            IridiumGatherers.Remove(unitObj);
        if (PalladiumGatherers.Contains(unitObj))
            PalladiumGatherers.Remove(unitObj);
        if (EezoGatherers.Contains(unitObj))
            EezoGatherers.Remove(unitObj);

    }
}
