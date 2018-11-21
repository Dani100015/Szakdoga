using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class XMLManager : MonoBehaviour {

    public static XMLManager ins;
    Game game;

    List<SolarSystem> Systems = new List<SolarSystem>();

    List<GameObject> SystemPrefabs = new List<GameObject>();
    List<GameObject> GalaxyStarPrefabs = new List<GameObject>();
    List<Transform> Celestials = new List<Transform>();
    List<GameObject> Units = new List<GameObject>();

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        ins = this;
        game = GameObject.Find("Game").GetComponent<Game>();
    }
    void Start()
    {
        Systems = game.Systems;
        SystemPrefabs = game.solarSystemPrefabs;
        GalaxyStarPrefabs = game.galaxyStarPrefabs;
        Units = game.Units;

        foreach (GameObject system in SystemPrefabs)
        {
            foreach (Transform celestials in system.transform.Find("Planets").GetComponentsInChildren<Transform>())
            {
                Celestials.Add(celestials);
            }
        }

        SetItemValues();
    }

    public ItemDatabase itemDB;

    void Update()
    {
        Units = game.Units;
    }
    public void SetItemValues()
    {
        #region Planet Items Set

        for (int i = 0; i < Celestials.Count; i++)
        {
            if (Celestials[i].GetComponent<ResourceObject>() == null)
            {
                PlanetItem planetItem = new PlanetItem();

                planetItem.celestialName = Celestials[i].name;
                planetItem.posX = Celestials[i].transform.position.x;
                planetItem.posY = Celestials[i].transform.position.y;
                planetItem.posZ = Celestials[i].transform.position.z;
                planetItem.systemName = Celestials[i].transform.parent.parent.name;

                itemDB.planets.Add(planetItem);
            }

        }
        #endregion

        #region Asteriod Items Set
        for (int i = 0; i < Celestials.Count; i++)
        {
            if (Celestials[i].GetComponent<ResourceObject>() != null)
            {
                AsteriodItem asteroidItem = new AsteriodItem();

                asteroidItem.celestialName = Celestials[i].name;
                asteroidItem.posX = Celestials[i].transform.position.x;
                asteroidItem.posY = Celestials[i].transform.position.y;
                asteroidItem.posZ = Celestials[i].transform.position.z;
                asteroidItem.systemName = Celestials[i].transform.parent.parent.name;

                itemDB.asteriods.Add(asteroidItem);
            }

        }
        #endregion

        #region  Systems Items Set
        for (int i = 0; i < SystemPrefabs.Count; i++)
        {
            SystemItem systemItem = new SystemItem();
            systemItem.systemName = SystemPrefabs[i].name;       
            for (int j = 0; j < SystemPrefabs[i].transform.Find("Planets").childCount; j++) //Naprendszer bolygóelemek nevei
            {
                systemItem.celestials.Add(SystemPrefabs[i].transform.Find("Planets").GetChild(j).name);
            }

            itemDB.systems.Add(systemItem);
        }
        #endregion

        #region Galaxy Stars Items Set
        for (int i = 0; i < GalaxyStarPrefabs.Count; i++)
        {
            StarItem starItem = new StarItem();

            starItem.starName = GalaxyStarPrefabs[i].name;
            starItem.posX = GalaxyStarPrefabs[i].transform.position.x;
            starItem.posY = GalaxyStarPrefabs[i].transform.position.y;
            starItem.posZ = GalaxyStarPrefabs[i].transform.position.z;

            itemDB.stars.Add(starItem);
        }
        #endregion

        #region Unit Items Set

        for (int i = 0; i < Units.Count; i++)
        {
            UnitItem unit = new UnitItem();

            unit.unitName = Units[i].name;
            unit.posX = Units[i].transform.position.x;
            unit.posY = Units[i].transform.position.y;
            unit.posZ = Units[i].transform.position.z;
            unit.starName = Units[i].transform.parent.parent.name;

            itemDB.units.Add(unit);
        }

        #endregion
    }
    public void SaveItems()
    {  
        XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
        FileStream stream = new FileStream(Application.dataPath + "/StreamingFiles/XML/item_data.xml", FileMode.Create);
        serializer.Serialize(stream,itemDB);
        stream.Close();
    }
    public void LoadItems()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
        FileStream stream = new FileStream(Application.dataPath + "/StreamingFiles/XML/item_data.xml", FileMode.Open);
        itemDB = (ItemDatabase)serializer.Deserialize(stream);

        stream.Close();

        ParameterWatcher.firstGameInit = true;
        ParameterWatcher.firstSolarSystemInit = true;
        ParameterWatcher.firstGalaxyInit = true;

        ParameterWatcher.isLoadedSolarSystem = true;
        ParameterWatcher.isLoadedGalaxy = true;

    SceneManager.LoadScene("SolarSystems_Teszt");
    }
    public void SetLoadedSolarSystem()
    {
        SystemPrefabs = new List<GameObject>();
        //Naprendszer beállítása
        for (int i = 0; i < Systems.Count; i++)
        {
            SystemPrefabs.Add(Instantiate((GameObject)Resources.Load("Prefabs/SolarSystem/SolarSystemPrefab1", typeof(GameObject))));
        }
        for (int i = 0; i < SystemPrefabs.Count; i++)
        {   
            SystemPrefabs[i].transform.position = Vector3.zero;
            SystemPrefabs[i].name = itemDB.systems[i].systemName;
            SystemPrefabs[i].AddComponent<GUI_CelestialToolTip>();
            SystemPrefabs[i].transform.SetParent(GameObject.Find("SolarSystems").transform);

            //Plantéták megfelelő számú beállításas
            GameObject[] planetPrefabs = Resources.LoadAll<GameObject>("Prefabs/SolarSystem/Planets") as GameObject[];
            for (int k = 0; k < itemDB.planets.Count; k++)
            {
                if (itemDB.planets[k].systemName == SystemPrefabs[i].name)
                {                  
                    int planetIndex = Random.Range(0, planetPrefabs.Length);
                    GameObject planet = Instantiate(planetPrefabs[planetIndex]);

                    planet.name = itemDB.planets[k].celestialName;
                    planet.transform.SetParent(SystemPrefabs[i].transform.Find("Planets"));
                    planet.transform.position = new Vector3(itemDB.planets[k].posX, itemDB.planets[k].posY, itemDB.planets[k].posZ);
                    planet.transform.localScale = new Vector3(planet.transform.localScale.x + 0.4f, planet.transform.localScale.y + 0.4f, planet.transform.localScale.z + 0.4f);
                    planet.AddComponent<GUI_CelestialToolTip>();
                }
            }

            //Asteroidák megfelelő számú beállítása
            GameObject[] asteriodsPrefabs = Resources.LoadAll<GameObject>("Prefabs/Asteroids") as GameObject[];
            for (int k = 0; k < itemDB.asteriods.Count; k++)
            {
                if (itemDB.asteriods[k].systemName == SystemPrefabs[i].name)
                {
                    int asteroidIndex = Random.Range(0, asteriodsPrefabs.Length);
                    GameObject asteroid = Instantiate(asteriodsPrefabs[asteroidIndex]);
                    asteroid.name = asteriodsPrefabs[asteroidIndex].name;

                    asteroid.transform.SetParent(SystemPrefabs[i].transform.Find("Planets"));
                    asteroid.GetComponent<ResourceObject>().Capacity = itemDB.asteriods[k].capacity;
                    asteroid.GetComponent<ResourceObject>().Type = itemDB.asteriods[k].type;
                    asteroid.transform.position = new Vector3(itemDB.asteriods[k].posX, itemDB.asteriods[k].posY, itemDB.asteriods[k].posZ);
                    asteroid.AddComponent<GUI_CelestialToolTip>();
                    asteroid.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
            }       

            SetSolarSystems.SystemPrefabs = SystemPrefabs;
            game.solarSystemPrefabs = SystemPrefabs;
        }
    }
    public void SetLoadedGalaxyStars()
    {

        GalaxyStarPrefabs = new List<GameObject>();
        GameObject[] starPrefabs = Resources.LoadAll<GameObject>("Prefabs/Galaxy") as GameObject[];
        
        for (int i = 0; i < Systems.Count; i++)
        {
            GameObject currentStar;
            currentStar = starPrefabs[Random.Range(0, 4)];

            GalaxyStarPrefabs.Add(Instantiate(currentStar));
        }
        for (int i = 0; i < GalaxyStarPrefabs.Count; i++)
        {
            GalaxyStarPrefabs[i].transform.SetParent(GameObject.Find("Galaxy").transform);
            GalaxyStarPrefabs[i].name = itemDB.stars[i].starName;
            GalaxyStarPrefabs[i].transform.position = new Vector3(itemDB.stars[i].posX, itemDB.stars[i].posY, itemDB.stars[i].posZ);
            GalaxyStarPrefabs[i].tag = "StarSystem";

            game.Systems[i].position = GalaxyStarPrefabs[i].transform.position;
        }

        SetGalaxySolarSystems.SystemPrefabs = GalaxyStarPrefabs;
        game.galaxyStarPrefabs = GalaxyStarPrefabs;
    }
    public void SetLoadedUnits()
    {
        Units = new List<GameObject>();
        string path = "Prefabs/Units";
        GameObject[] LoadedUnits = Resources.LoadAll<GameObject>(path);

        for (int i = 0; i < itemDB.units.Count; i++)
        {
            GameObject temp = null;
            for (int j = 0; j < LoadedUnits.Length; j++)
            {
                if (LoadedUnits[i].name == itemDB.units[i].unitName)
                {
                    temp = Instantiate(LoadedUnits[i]);
                    temp.name = (LoadedUnits[i] as GameObject).name;
                    break;
                }
            }
            temp.GetComponent<Unit>().ScreenPos = new Vector2(itemDB.units[i].posX, itemDB.units[i].posZ);
            temp.transform.position = new Vector3(itemDB.units[i].posX, itemDB.units[i].posY, itemDB.units[i].posZ );
            Debug.Log(itemDB.units[i].starName);
            temp.transform.SetParent(SystemPrefabs.Find(x => x.name == itemDB.units[i].starName).transform.Find("Units"));
            temp.gameObject.tag = "Unit";           
         //   temp.hideFlags = HideFlags.HideInHierarchy;
            temp.SetActive(true);
            Units.Add(temp);
        }
    }
}

#region Item Classes

[System.Serializable]
public class PlanetItem 
{
    public string celestialName;
    public float posX, posY, posZ;
    public string systemName;
    //public string prefabName;
}

[System.Serializable]
public class AsteriodItem
{
    public string celestialName;
    public float posX, posY, posZ;
    public string systemName;

    public float capacity;
    public resourceType type;
}

[System.Serializable]
public class SystemItem
{
    public string systemName;
    public List<string> celestials = new List<string>();
}

[System.Serializable]
public class StarItem
{
    public string starName;
    public float posX, posY, posZ;
}

[System.Serializable]
public class UnitItem
{
    public string unitName;
    public float posX, posY, posZ;
    public string starName;
}

#endregion

[System.Serializable]
public class ItemDatabase
{
    public List<PlanetItem> planets = new List<PlanetItem>();
    public List<AsteriodItem> asteriods = new List<AsteriodItem>();
    public List<SystemItem> systems = new List<SystemItem>();
    public List<StarItem> stars = new List<StarItem>();
    public List<UnitItem> units = new List<UnitItem>();
}