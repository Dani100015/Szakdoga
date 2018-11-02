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
        solarSystem.transform.Find("Star").gameObject.SetActive(true);

        currentSystemPrefab = solarSystem;
    }

    public void GenerateSolarSystemPrefabs(int starCount)
    {
        for (int i = 0; i < Systems.Count; i++)
        {
            SystemPrefabs.Add(Instantiate((GameObject)Resources.Load("Prefabs/SolarSystem/SolarSystemPrefab", typeof(GameObject))));
            SystemPrefabs[i].transform.position = Systems[i].position;
            SystemPrefabs[i].name = Systems[i].Name;
            //Systems[i].solarSystemGObject = SystemPrefabs[i];
        }
    }
}

