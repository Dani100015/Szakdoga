using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

class SetSolarSystems : MonoBehaviour
{
    //Scriptek
    Game game;
    SetSolarSystems setSystems;
    XMLManager xmlManager;

    public GameObject startSystemPrefab;
    public GameObject currentSystemPrefab;

    public static List<SolarSystem> Systems;
    public static List<GameObject> SystemGObjects = new List<GameObject>();
    public List<GameObject> units;

    bool fromGalaxy;
    public AstarData active;

    void Awake()
    {
        fromGalaxy = false;
        game = GameObject.Find("Game").GetComponent<Game>();
        setSystems = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();
        xmlManager = new XMLManager();
    }

    /// <summary>
    /// Várunk addig, hogy minden kellő információ meglegyen, a naprendszer keringési pálya kirazolásához.
    /// </summary>
    /// <returns></returns>
    IEnumerator DrawSolarSystemLines()
    {
        yield return new WaitForSeconds(0.1f);

        startSystemPrefab = SystemGObjects.Find(x => x.name == game.startSolarSystem.Name);
        currentSystemPrefab = startSystemPrefab;

        DisappearOtherSolarSystem(currentSystemPrefab);
    }
    void Start()
    {
        //Csak egyszer fut le, alapbeállítások, kezdőbeállítások
        if (ParameterWatcher.firstSolarSystemInit == true)
        {
            Systems = game.Systems;

            GenerateSolarSystemPrefabs(game.Systems.Count);
            StartCoroutine(DrawSolarSystemLines());

            ParameterWatcher.firstSolarSystemInit = false;
        }
    }
    void Update()
    {
        if (currentSystemPrefab != null && game.currentSolarSystem.Name != currentSystemPrefab.name)
        {
            fromGalaxy = true;
            DisappearOtherSolarSystem(SystemGObjects.Where(x => x.name == game.currentSolarSystem.Name).First());
            currentSystemPrefab = SystemGObjects.Find(x => x.name == game.currentSolarSystem.Name);
        }
    }
    public void DisappearOtherSolarSystem(GameObject currentSolarSystem)
    {
        foreach (GameObject solar in SystemGObjects)
        {
            foreach (Renderer mesh in solar.GetComponentsInChildren<Renderer>())
            {
                mesh.enabled = false;       
            }
            if (solar.name == game.currentSolarSystem.Name)
            {
                currentSolarSystem.transform.Find("Star").gameObject.SetActive(true);
                currentSolarSystem.transform.Find("Star").GetComponent<MeshRenderer>().enabled = true;
            }
            else
                solar.transform.Find("Star").gameObject.SetActive(false);
        }
        AstarPath.active = game.GetComponent<AstarPath>();
        SetCurrentSolarSystem(currentSolarSystem);
    }
    void SetCurrentSolarSystem(GameObject currentSolarSytem)
    {
        foreach (Renderer mesh in currentSolarSytem.GetComponentsInChildren<Renderer>())
        {
            mesh.enabled = true;
        }
    }
    /// <summary>
    /// Naprendszer GObjectek generálása/lerakása
    /// </summary>
    /// <param name="starCount"></param>
    public void GenerateSolarSystemPrefabs(int starCount)
    {
        if (!ParameterWatcher.isLoadedSolarSystem)
        {
            for (int i = 0; i < Systems.Count; i++)
            {
                #region Naprendszer generálás
                SystemGObjects.Add(Instantiate((GameObject)Resources.Load("Prefabs/SolarSystem/SolarSystemPrefab1", typeof(GameObject))));
                SystemGObjects[i].transform.position = Vector3.zero;
                SystemGObjects[i].name = Systems[i].Name;
                SystemGObjects[i].AddComponent<GUI_CelestialToolTip>();
                #endregion
                #region Planéták generálása 
                GameObject[] planetPrefabs = Resources.LoadAll<GameObject>("Prefabs/SolarSystem/Planets") as GameObject[];
                int planetNumber = Random.Range(3, 7);
                for (int j = 0; j < planetNumber; j++)
                {
                    int planetIndex = Random.Range(0, planetPrefabs.Length);
                    GameObject planet = Instantiate(planetPrefabs[planetIndex]);
                    planet.transform.SetParent(SystemGObjects[i].transform.Find("Planets"));
                    planet.transform.position = new Vector3(Random.Range(-150, 150), 0, Random.Range(-250, 250));
                    planet.transform.localScale = new Vector3(planet.transform.localScale.x + 0.4f, planet.transform.localScale.y + 0.4f, planet.transform.localScale.z + 0.4f);
                    planet.AddComponent<GUI_CelestialToolTip>();
                }
                #endregion
                #region Asteroidák generálsa
                GameObject[] asteriodsPrefabs = Resources.LoadAll<GameObject>("Prefabs/Asteroids") as GameObject[];
                int asteriodsNumber = Random.Range(3, 7);
                for (int j = 0; j < asteriodsNumber; j++)
                {
                    int asteriodIndex = Random.Range(0, asteriodsPrefabs.Length);
                    GameObject asteroid = Instantiate(asteriodsPrefabs[asteriodIndex]);
                    asteroid.name = asteriodsPrefabs[asteriodIndex].name;

                    asteroid.transform.SetParent(SystemGObjects[i].transform.Find("Planets"));
                    asteroid.GetComponent<ResourceObject>().Capacity = Random.Range(10000, 50000);
                    asteroid.transform.position = new Vector3(Random.Range(-250, 250), 0, Random.Range(-250, 250));
                    asteroid.AddComponent<GUI_CelestialToolTip>();
                    asteroid.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                #endregion

                SystemGObjects[i].transform.SetParent(GameObject.Find("SolarSystems").transform);
                game.solarSystemPrefabs = SystemGObjects;
            }

            #region Init

            //Játékosok inicializálása


            //Egységek betöltése és játékosokhoz rendelése
            string path = "Prefabs/Units";
            object[] LoadedUnits = Resources.LoadAll(path);
            List<GameObject> Units = new List<GameObject>();
            for (int i = 0; i < LoadedUnits.Length; i++)
            {
                GameObject temp = Instantiate(LoadedUnits[i] as GameObject);
                if (GameObject.Find("SolarSystems") != null)
                    temp.transform.SetParent(GameObject.Find("SolarSystems").transform.Find(game.currentSolarSystem.Name).transform.Find("Units"));
                temp.gameObject.tag = "Unit";
                temp.name = (LoadedUnits[i] as GameObject).name;
                temp.hideFlags = HideFlags.HideInHierarchy;
                temp.SetActive(false);
                temp.GetComponent<BoxCollider>().enabled = true;
                Units.Add(temp);
            }

            if (Units.Count > 0)
            {
                for (int i = 0; i < Units.Count; i++)
                {
                    GameObject unit = Units[i] as GameObject;
                    Unit unitobj = unit.GetComponent<Unit>();
                    foreach (Player p in Game.players)
                    {
                        if (unitobj.Race == p.species)
                            p.BuildableUnits.Add(unit);
                    }
                }
            }

            Game.SharedIcons = Resources.LoadAll("Icons/Shared");
            #endregion
            #region Induló elemek
            //A pályán levő egységeket/épületeket a tulajdonosuk listáihoz rendeli
            var goArray = FindObjectsOfType(typeof(GameObject));
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < goArray.Length; i++)
            {
                GameObject currentObject = goArray[i] as GameObject;
                if (currentObject.GetComponent<Unit>() != null || currentObject.GetComponent<Structure>() != null)
                {
                    Game.players.Where(x => x.empireName.Equals(currentObject.GetComponent<Unit>().Owner)).SingleOrDefault().units.Add(currentObject);
                }

            }
            #endregion
        }
        else
        {
            Debug.Log("Loaded");
            xmlManager.SetLoadedSolarSystem();
            xmlManager.SetLoadedUnits();
          

            ParameterWatcher.isLoadedSolarSystem = false;
        }

    }
}

