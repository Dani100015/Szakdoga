using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


class SetGalaxySolarSystems : MonoBehaviour {

    Game game;

    GameObject[] starPrefabs;

    public List<GameObject> SystemPrefabs = new List<GameObject>();
    public List<SolarSystem> Systems = new List<SolarSystem>();

    public GameObject startSystemGObject;
    public GameObject currentSystemGObject;

    List<LineRenderer> neighbourLine;
    GameObject lineContainer;

    void Awake()
    {
       
    }
    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        Systems = game.Systems;

        startSystemGObject = SystemPrefabs.Find(x => x.name == game.startSolarSystem.Name);
        currentSystemGObject = startSystemGObject;

        starPrefabs = Resources.LoadAll<GameObject>("Prefabs/Galaxy") as GameObject[];
        lineContainer = GameObject.Find("LineContainer");

        GenerateGalaxyPrefabs();
        GenerateSystemRelations();
        GenerateSystemRelationsLine();
            
    }
    void GenerateGalaxyPrefabs()
    {
        GameObject currentSystem;
        for (int i = 0; i < Systems.Count; i++)
        {
            currentSystem = starPrefabs[Random.Range(0,4)];

            currentSystem.name = Systems[i].Name;
            currentSystem.transform.position = Systems[i].position;
            currentSystem.tag = "StarSystem";

            SystemPrefabs.Add(Instantiate(currentSystem));


        }
    }
    void GenerateSystemRelations()
    {
        //Szomszéd viszonyság generálás
        for (int i = 0; i < Systems.Count; i++)
        {
            Systems[i].neighbourSystems = new List<SolarSystem>();

            int neigbourCount = Random.Range(1, Systems.Count-1);
            for (int j = 0; j < neigbourCount; j++)
            {
                int rndIndex = Random.Range(0, Systems.Count);

                if (Systems[rndIndex].Name != Systems[i].Name)
                {
                    Systems[i].neighbourSystems.Add(Systems[rndIndex]);
                }

               // Debug.Log(Systems[i].Name + ", sz: " + Systems[i].neighbourSystems[j].Name);
            }
        }


    }
    void GenerateSystemRelationsLine()
    {

        neighbourLine = new List<LineRenderer>();
        //Szomszéd vonal generálás
        for (int i = 0; i < Systems.Count; i++)
        {
            for (int j = 0; j < Systems[i].neighbourSystems.Count; j++)
            {
                GameObject gObject = new GameObject("LineObject");
                LineRenderer line = gObject.AddComponent<LineRenderer>();
                line.transform.SetParent(lineContainer.transform);

                line.startWidth = 0.2f;
                line.endWidth = 0.2f;
                line.positionCount = 2;              

                line.SetPosition(0, Systems[i].position);
                line.SetPosition(1, Systems[i].neighbourSystems[j].position);

                neighbourLine.Add(new LineRenderer());
            }
        }
    }

}
