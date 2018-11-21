using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


class SetGalaxySolarSystems : MonoBehaviour {

    Game game;
    XMLManager xmlManager;

    GameObject[] starPrefabs;

    public static List<GameObject> SystemPrefabs = new List<GameObject>();
    public static List<SolarSystem> Systems = new List<SolarSystem>();

    public GameObject startSystemGObject;
    public GameObject currentSystemGObject;

    List<LineRenderer> neighbourLine;
    GameObject lineContainer;

    void Start()
    {
        if (ParameterWatcher.firstGalaxyInit)
        {
            game = GameObject.Find("Game").GetComponent<Game>();
            xmlManager = GameObject.Find("XMLManager").GetComponent<XMLManager>();
            Systems = game.Systems;

            GenerateGalaxyPrefabs();

            startSystemGObject = SystemPrefabs.Find(x => x.name == game.startSolarSystem.Name);
            currentSystemGObject = startSystemGObject;

            lineContainer = GameObject.Find("LineContainer");
            GenerateSystemRelationsLine();


            ParameterWatcher.firstGalaxyInit = false;
        }

    }
    void GenerateGalaxyPrefabs()
    {
        if (!ParameterWatcher.isLoadedGalaxy)
        {
            starPrefabs = Resources.LoadAll<GameObject>("Prefabs/Galaxy") as GameObject[];
            GameObject currentSystem;
            for (int i = 0; i < Systems.Count; i++)
            {
                currentSystem = starPrefabs[Random.Range(0, 4)];

                currentSystem.name = Systems[i].Name;
                currentSystem.transform.position = Systems[i].position + new Vector3(1000, 0, 1000);
                currentSystem.tag = "StarSystem";

                SystemPrefabs.Add(Instantiate(currentSystem));
            }
            for (int i = 0; i < SystemPrefabs.Count; i++)
            {
                SystemPrefabs[i].transform.SetParent(GameObject.Find("Galaxy").transform);
            }
            for (int i = 0; i < SystemPrefabs.Count; i++)
            {
                SystemPrefabs[i].name = Systems[i].Name;
            }
            game.galaxyStarPrefabs = SystemPrefabs;
        }
        else
        {
            xmlManager.SetLoadedGalaxyStars();
            ParameterWatcher.isLoadedGalaxy = false;
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

                line.startWidth = 0.3f;
                line.endWidth = 0.3f;
                line.positionCount = 2;

                if (ParameterWatcher.isLoadedGalaxy)
                {
                    line.SetPosition(0, Systems[i].position);
                    line.SetPosition(1, Systems[i].neighbourSystems[j].position);
                }
                else
                {
                    line.SetPosition(0, Systems[i].position + new Vector3(1000, 0, 1000));
                    line.SetPosition(1, Systems[i].neighbourSystems[j].position + new Vector3(1000, 0, 1000));
                }
               

                //line.transform.SetParent();

                line.material.color = Color.cyan;
                line.gameObject.layer = 15;

                neighbourLine.Add(line);
            }
        }
    }

}
