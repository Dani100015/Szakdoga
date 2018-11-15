using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Pathfinding;

class SetSolarSystems : MonoBehaviour {

    Game game;
    SetSolarSystems setSystems;

    public GameObject startSystemPrefab;
    public GameObject currentSystemPrefab;

    public List<SolarSystem> Systems;
    public List<GameObject> SystemPrefabs = new List<GameObject>();

    bool fromGalaxy;
    public AstarData active;

    void Awake()
    {
        fromGalaxy = false;
        game = GameObject.Find("Game").GetComponent<Game>();
        setSystems = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();
    }
    void Start ()
    {
        Systems = game.Systems;
        if (ParameterWatcher.firstSolarSystemInit)
        {
            GenerateSolarSystemPrefabs(game.Systems.Count);
            startSystemPrefab = SystemPrefabs.Find(x => x.name == game.startSolarSystem.Name);
            currentSystemPrefab = startSystemPrefab;
            InitialSolarSystems(currentSystemPrefab);
            ParameterWatcher.firstSolarSystemInit = false;
        }
                    

    }

	void Update () {

        if (game.currentSolarSystem.Name != currentSystemPrefab.name)
        {
            fromGalaxy = true;
            InitialSolarSystems(SystemPrefabs.Find(x => x.name == game.currentSolarSystem.Name));
        }
    }
        
    public void InitialSolarSystems(GameObject solarSystem)
    {    
        for (int i = 0; i < Systems.Count; i++)
        {
            foreach (GameObject solar in SystemPrefabs)
            {
                foreach (MeshRenderer mesh in solar.GetComponentsInChildren<MeshRenderer>())
                {
                    mesh.enabled = false;               
                }
                solar.transform.Find("Star").gameObject.SetActive(false);
                
                foreach (LineRenderer line in solar.transform.Find("LineContainer").GetComponentsInChildren<LineRenderer>())
                {
                    line.enabled = false;
                }
            }      
        }       
        SetCurrentSolarSystem(solarSystem);                                  
    }
    
    void SetCurrentSolarSystem(GameObject solarSystem)
    {

        foreach (MeshRenderer mesh in solarSystem.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }
        foreach (LineRenderer line in solarSystem.transform.Find("LineContainer").GetComponentsInChildren<LineRenderer>())
        {
            line.enabled = true;
        }
        solarSystem.transform.Find("Star").gameObject.SetActive(true);
        solarSystem.transform.Find("Star").GetComponent<MeshRenderer>().enabled = true;
        AstarPath.active = game.GetComponent<AstarPath>();
    }

    public void GenerateSolarSystemPrefabs(int starCount)
    {
        //AstarData data = AstarPath.active.data;
        //int width = 300;
        //int depth = 300;
        //float nodeSize = 2;
        ////Pathfindinghoz hozzáadunk egy új gráfot
        //GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;
        ////Beállítjuk annak középpontját, méreteit
        //gg.center = new Vector3(0, -5, 0);
        //gg.SetDimensions(width, depth, nodeSize);
        ////Szkennelünk
        //AstarPath.active.Scan();

        for (int i = 0; i < Systems.Count; i++)
        {
            SystemPrefabs.Add(Instantiate((GameObject)Resources.Load("Prefabs/SolarSystem/SolarSystemPrefab1", typeof(GameObject))));
            SystemPrefabs[i].transform.position = Vector3.zero;
            SystemPrefabs[i].name = Systems[i].Name;
            SystemPrefabs[i].AddComponent<GUI_CelestialToolTip>();

            GameObject[] planetPrefabs = Resources.LoadAll<GameObject>("Prefabs/SolarSystem/Planets") as GameObject[];
            int planetNumber = Random.Range(3, 7);
            for (int j = 0; j < planetNumber; j++)
            {
                int planetIndex = Random.Range(0, planetPrefabs.Length);
                GameObject planet = Instantiate(planetPrefabs[planetIndex]);
                planet.transform.SetParent(SystemPrefabs[i].transform.Find("Planets"));
                planet.transform.position = new Vector3(Random.Range(-150,150),0, Random.Range(-250, 250));
                planet.transform.localScale = new Vector3(planet.transform.localScale.x+0.4f, planet.transform.localScale.y + 0.4f, planet.transform.localScale.z + 0.4f);
                planet.AddComponent<GUI_CelestialToolTip>();
            }

            GameObject[] asteriodsPrefabs = Resources.LoadAll<GameObject>("Prefabs/Asteroids") as GameObject[];
            int asteriodsNumber = Random.Range(3, 7);
            Debug.Log(asteriodsNumber);
            for (int j = 0; j < asteriodsNumber; j++)
            {
                int asteriodIndex = Random.Range(0, asteriodsPrefabs.Length);
                GameObject asteroid = Instantiate(asteriodsPrefabs[asteriodIndex]);
                asteroid.transform.SetParent(SystemPrefabs[i].transform.Find("Planets"));
                asteroid.GetComponent<ResourceObject>().Capacity = Random.Range(100, 1000);
                asteroid.transform.position = new Vector3(Random.Range(-250, 250), 0, Random.Range(-250, 250));
                asteroid.AddComponent<GUI_CelestialToolTip>();
            }

            SystemPrefabs[i].transform.SetParent(GameObject.Find("SolarSystems").transform);
        }      
    }
}

