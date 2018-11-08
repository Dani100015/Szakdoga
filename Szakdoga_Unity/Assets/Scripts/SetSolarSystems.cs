using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class SetSolarSystems : MonoBehaviour {

    Game game;
    SetSolarSystems setSystems;

    public GameObject startSystemPrefab;
    public GameObject currentSystemPrefab;

    public List<SolarSystem> Systems;
    public List<GameObject> SystemPrefabs = new List<GameObject>();

    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        setSystems = gameObject.GetComponent<SetSolarSystems>();

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

        currentSystemPrefab = solarSystem;
    }

    public void GenerateSolarSystemPrefabs(int starCount)
    {
        for (int i = 0; i < Systems.Count; i++)
        {
            SystemPrefabs.Add(Instantiate((GameObject)Resources.Load("Prefabs/SolarSystem/SolarSystemPrefab", typeof(GameObject))));
            SystemPrefabs[i].transform.position = Vector3.zero;
            SystemPrefabs[i].name = Systems[i].Name;

            GameObject asteroid = Instantiate((GameObject)Resources.Load("Prefabs/Palladium Asteroid", typeof(GameObject)));
            asteroid.GetComponent<ResourceObject>().Capacity = Random.Range(100, 1000);
            asteroid.transform.position = new Vector3(200, 0, 100);
            asteroid.transform.SetParent(SystemPrefabs[i].transform.Find("Planets").transform);
            //asteroid.GetComponent<ResourceObject>().Type = resourceType.
            //Systems[i].solarSystemGObject = SystemPrefabs[i];
        }
    }
}

