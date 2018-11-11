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

        for (int i = 0; i < SystemPrefabs.Count; i++)
        {
            SystemPrefabs[i].name = Systems[i].Name;
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

                Debug.Log(line.transform.parent.name);

                line.startWidth = 0.3f;
                line.endWidth = 0.3f;
                line.positionCount = 2;              

                line.SetPosition(0, Systems[i].position);
                line.SetPosition(1, Systems[i].neighbourSystems[j].position);              

                //line.transform.SetParent();

                line.material.color = Color.cyan;
                line.gameObject.layer = 15;

                neighbourLine.Add(line);
            }
        }
    }

}
