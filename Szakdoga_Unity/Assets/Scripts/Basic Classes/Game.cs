using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

class Game : MonoBehaviour
{

    public static Game game;

    List<SolarSystem> solars;
    public static List<Player> players;
    //List<Team> teams;

    public static Player currentPlayer;
    public static Player player2;

    public static object[] SharedIcons;

    public List<SolarSystem> Systems;

    public List<GameObject> galaxyStarPrefabs;
    public List<GameObject> solarSystemPrefabs;

    public SolarSystem startSolarSystem;
    public SolarSystem currentSolarSystem;

    public int starCount;

    public List<Tech> playerTechList = new List<Tech>();

    public SetSolarSystems setSystems;
    public GameStartOptions gameStart;

    public static bool GalaxyView;

    static public Camera mainCamera;

    public bool fromGalaxy;

    public List<GameObject> units = new List<GameObject>();
    void Awake()
    {

        if (ParameterWatcher.firstGameInit)
        {

            starCount = 5;

            fromGalaxy = false;

            GenerateSolarSystems(starCount);
            GenerateSystemRelations();

            startSolarSystem = Systems[0];
            currentSolarSystem = startSolarSystem;

            initTechTree();

            Game.mainCamera = Camera.main;

            Game.game = transform.GetComponent<Game>();

            ParameterWatcher.firstGameInit = false;

        }

        #region Induló elemek
        //A pályán levő egységeket/épületeket a tulajdonosuk listáihoz rendeli
        var goArray = FindObjectsOfType(typeof(GameObject));
        List<GameObject> goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            GameObject currentObject = goArray[i] as GameObject;
            if (currentObject.GetComponent<Unit>() != null || currentObject.GetComponent<Structure>() != null)
            {
                players.Where(x => x.empireName.Equals(currentObject.GetComponent<Unit>().Owner)).SingleOrDefault().units.Add(currentObject);
            }

        }
        #endregion
    }

    void Start()
    {

        #region Init

        //Játékosok inicializálása
        players = new List<Player>();

        currentPlayer = new Player(10000, 10000, 10000, "Peti", Species.Human);
        player2 = new Player(10000, 10000, 10000, "Sanyi", Species.Human);

        currentPlayer.enemies.Add(player2);
        player2.enemies.Add(currentPlayer);

        players.Add(currentPlayer);
        players.Add(player2);

        //Egységek betöltése és játékosokhoz rendelése
        string path = "Prefabs/Units";
        object[] LoadedUnits = Resources.LoadAll(path);
        List<GameObject> Units = new List<GameObject>();
        for (int i = 0; i < LoadedUnits.Length; i++)
        {
            GameObject temp = Instantiate(LoadedUnits[i] as GameObject);
            temp.transform.SetParent(GameObject.Find("SolarSystems").transform.Find(currentSolarSystem.Name).transform.Find("Units"));
            temp.gameObject.tag = "Unit";
            Units.Add(temp);
        }

        if (Units.Count > 0)
        {
            for (int i = 0; i < Units.Count; i++)
            {
                GameObject unit = Units[i] as GameObject;
                Unit unitobj = unit.GetComponent<Unit>();
                foreach (Player p in players)
                {
                    if (unitobj.Race == p.species)
                        p.BuildableUnits.Add(unit);
                }
            }

            units = Units;
        }

        SharedIcons = Resources.LoadAll("Icons/Shared");
        #endregion


    }

    void Update()
    {
        
    }

    // void CheckForWin
    // void CheckForLose
    // void setNewGame
    // void EndGame

    public void GenerateSolarSystems(int starCount)
    {
        //X számú rendszer generálás
        Systems = new List<SolarSystem>();

        Vector3 randomV3;
        Vector3 minDistance = new Vector3(10, 0, 10);

        for (int i = 0; i < starCount; i++)
        {
            randomV3 = new Vector3((float)Random.Range(10, 500), (float)13, (float)Random.Range(10, 500));
            Systems.Add(new SolarSystem(randomV3, i + "._SOLARSYSTEM", null, null));
        }
    }

    void GenerateSystemRelations()
    {
        for (int i = 0; i < Systems.Count; i++)
        {
            if (Systems[i].neighbourSystems == null)
            {
                Systems[i].neighbourSystems = new List<SolarSystem>();
            }

            int neigbourCount;
            if (Systems[i].neighbourSystems != null)
            {
                neigbourCount = Random.Range(Systems[i].neighbourSystems.Count, Systems.Count);
            }
            else
            {
                neigbourCount = Random.Range(1, Systems.Count);
            }

            while (Systems[i].neighbourSystems.Count != neigbourCount)
            {
                int rndIndex = Random.Range(0, Systems.Count);

                if (!Systems[i].neighbourSystems.Contains(Systems[rndIndex]) && Systems[rndIndex].Name != Systems[i].Name)
                {
                    Systems[i].neighbourSystems.Add(Systems[rndIndex]);
                    if (Systems[rndIndex].neighbourSystems == null)
                    {
                        Systems[rndIndex].neighbourSystems = new List<SolarSystem>();
                    }
                    Systems[rndIndex].neighbourSystems.Add(Systems[i]);
                }


            }
        }
    }

    public static void ResearchEffects(Player player, Tech tech)
    {
        switch (tech.name)
        {
            case "FerroUraniumSlogs":
            case "EfficientRailTracks":
            case "LowerProjectiveMass":
                IncreaseAttack(player);
                break;

            case "HawkingCarrier":
            case "YorkDreadnought":
            case "EinsteinCarrier":
                break;

            case "TurianDesignElements":
            case "HighStressMaterials":
            case "QuarianMiracleWorkers":
                IncreaseMaxHealth(player);
                break;

            case "KroganArmorDesign":
            case "SilarisDistribution":
                IncreaseArmor(player);
                break;

            case "RapidHarvesting":
            case "QualityControl":
            case "AncientMethods":
                IncreaseGatherSpeed(player);
                break;

            case "CSecConscription":
            case "KroganMercenaries":
            case "AllianceDefenseBoard":
            case "AsariHighCommand":
            case "CitadelDefenceBoard":
                IncreaseMaxPopulation(player);
                break;

        }
    }

    public static void IncreaseMaxPopulation(Player player)
    {
        player.MaxPopulation += 30;
    }
    public static void IncreaseGatherSpeed(Player player)
    {
        for (int i = 0; i < player.units.Count; i++)
        {
            if (player.units[i].GetComponent<Unit>() != null)
                player.units[i].GetComponent<Unit>().GatherSpeed += 1f;
        }
        for (int i = 0; i < player.BuildableUnits.Count; i++)
        {
            if (player.BuildableUnits[i].GetComponent<Unit>() != null)
                player.BuildableUnits[i].GetComponent<Unit>().GatherSpeed += 1f;
        }       
    }

    public static void IncreaseMaxHealth(Player player)
    {       
        for (int i = 0; i < player.units.Count; i++)
        {
            Debug.Log(player.units[i].name);
            if (player.units[i].GetComponent<Unit>() != null)
            {
                Debug.Log(player.units[i].GetComponent<Unit>().maxHealth);
                player.units[i].GetComponent<Unit>().maxHealth += player.units[i].GetComponent<Unit>().HealthIncrement;
                Debug.Log(player.units[i].GetComponent<Unit>().maxHealth);
                player.units[i].GetComponent<Unit>().currentHealth += player.units[i].GetComponent<Unit>().HealthIncrement;
            }
        }
        for (int i = 0; i < player.BuildableUnits.Count; i++)
        {
            if (player.BuildableUnits[i].GetComponent<Unit>() != null)
            {
                Undo.RecordObject(player.BuildableUnits[i], "Életerő növelés");
                player.BuildableUnits[i].GetComponent<Unit>().maxHealth += player.BuildableUnits[i].GetComponent<Unit>().HealthIncrement;
                player.BuildableUnits[i].GetComponent<Unit>().currentHealth += player.BuildableUnits[i].GetComponent<Unit>().HealthIncrement;
            }
        }
    }

    public static void IncreaseArmor(Player player)
    {
        for (int i = 0; i < player.units.Count; i++)
        {
            if (player.units[i].GetComponent<Unit>() != null && player.units[i].GetComponent<Unit>().isGatherer)
                player.units[i].GetComponent<Unit>().Armor += player.BuildableUnits[i].GetComponent<Unit>().ArmorIncrement;
        }
        for (int i = 0; i < player.BuildableUnits.Count; i++)
        {
            if (player.BuildableUnits[i].GetComponent<Unit>() != null && player.BuildableUnits[i].GetComponent<Unit>().isGatherer)
                player.BuildableUnits[i].GetComponent<Unit>().Armor += player.BuildableUnits[i].GetComponent<Unit>().ArmorIncrement;
        } 
    }

    public static void IncreaseAttack(Player player)
    {
        for (int i = 0; i < player.units.Count; i++)
        {
            if (player.units[i].GetComponent<Unit>() != null)
                player.units[i].GetComponent<Unit>().attackDamage += player.BuildableUnits[i].GetComponent<Unit>().AttackIncrement;
        }
        for (int i = 0; i < player.BuildableUnits.Count; i++)
        {
            if (player.BuildableUnits[i].GetComponent<Unit>() != null)
                player.BuildableUnits[i].GetComponent<Unit>().attackDamage += player.BuildableUnits[i].GetComponent<Unit>().AttackIncrement;
        }
    }

    public void initTechTree()
    {
        playerTechList = new List<Tech>();
        for (int i = 0; i < Tech.techList.Count; i++)
        {
            playerTechList.Add(Tech.techList[i]);
        }

    }
}
