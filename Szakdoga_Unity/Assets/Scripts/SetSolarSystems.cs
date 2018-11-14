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
        GenerateSolarSystemPrefabs(game.Systems.Count);
        startSystemPrefab = SystemPrefabs.Find(x => x.name == game.startSolarSystem.Name);
        currentSystemPrefab = startSystemPrefab;
        InitialSolarSystems(currentSystemPrefab); 
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
        foreach (LineRenderer line  in solarSystem.transform.Find("LineContainer").GetComponentsInChildren<LineRenderer>())
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
            SystemPrefabs.Add(Instantiate((GameObject)Resources.Load("Prefabs/SolarSystem/SolarSystemPrefab", typeof(GameObject))));
            SystemPrefabs[i].transform.position = Vector3.zero;
            SystemPrefabs[i].name = Systems[i].Name;                               

            GameObject asteroid = Instantiate((GameObject)Resources.Load("Prefabs/Palladium Asteroid", typeof(GameObject)));
            asteroid.GetComponent<ResourceObject>().Capacity = Random.Range(100, 1000);
            asteroid.transform.position = new Vector3(200, 0, 100);
            asteroid.transform.SetParent(SystemPrefabs[i].transform.Find("Planets").transform);
        }

        List<GameObject> planets;
        for (int i = 0; i < SystemPrefabs.Count;  i++)
        {
            planets = new List<GameObject>();
            
        }        
    }
}

