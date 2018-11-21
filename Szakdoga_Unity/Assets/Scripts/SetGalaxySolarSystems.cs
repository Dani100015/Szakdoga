using System.Collections.Generic;
using UnityEngine;


class SetGalaxySolarSystems : MonoBehaviour {

    //Scriptek
    Game game;
    XMLManager xmlManager;

    public static List<GameObject> GalaxyGObjects = new List<GameObject>();
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

            startSystemGObject = GalaxyGObjects.Find(x => x.name == game.startSolarSystem.Name);
            currentSystemGObject = startSystemGObject;

            lineContainer = GameObject.Find("LineContainer");
            GenerateSystemRelationsLine();


            ParameterWatcher.firstGalaxyInit = false;
        }

    }
    void GenerateGalaxyPrefabs()
    {
        //Megvizsgáljuk, hogy mentett Galaxis objecteket akarunk-e betölteni, vagy csak alap véletlenszerű generálás
        if (!ParameterWatcher.isLoadedGalaxy)
        {
            GameObject[] galaxyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Galaxy") as GameObject[];
            GameObject currentGalaxyGObject;

            for (int i = 0; i < Systems.Count; i++)
            {
                currentGalaxyGObject = galaxyPrefabs[Random.Range(0, 4)];
                currentGalaxyGObject.name = Systems[i].Name;
                currentGalaxyGObject.transform.position = Systems[i].position + new Vector3(1000, 0, 1000);
                currentGalaxyGObject.tag = "StarSystem";

                GalaxyGObjects.Add(Instantiate(currentGalaxyGObject));
            }
            //GObject szülő beállítás
            for (int i = 0; i < GalaxyGObjects.Count; i++)  
            {
                GalaxyGObjects[i].transform.SetParent(GameObject.Find("Galaxy").transform);
                GalaxyGObjects[i].name = Systems[i].Name;
            }
            game.galaxyStarPrefabs = GalaxyGObjects;    
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

                line.transform.SetParent(lineContainer.transform);
                line.startWidth = 0.3f;
                line.endWidth = 0.3f;
                line.positionCount = 2;
                line.material.color = Color.cyan;
                line.gameObject.layer = 15;

                neighbourLine.Add(line);
            }
        }
    }

}
