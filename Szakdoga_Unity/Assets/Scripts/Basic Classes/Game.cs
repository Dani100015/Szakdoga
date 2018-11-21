using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

class Game : MonoBehaviour
{
    #region Változók

    //Script hivatkozások
    public static Game game;
    public SetSolarSystems setSystems;
    public GameStartOptions gameStart;

    //Aktuális cameranézet
    static public Camera mainCamera;

    //Players
    public static Player currentPlayer;
    public static Player player2;
    public static List<Player> players;

    //Icons
    public static object[] SharedIcons;

    //Unit,Naprendszer listák
    public List<SolarSystem> Systems;
    public List<GameObject> galaxyStarPrefabs;
    public List<GameObject> solarSystemPrefabs;
    public List<GameObject> Units = new List<GameObject>();

    //Kezdő naprendszer
    public SolarSystem startSolarSystem;

    //A jelenlegi naprendszer
    public SolarSystem currentSolarSystem;

    //Tech inicializálás
    public List<Tech> playerTechList = new List<Tech>();

    //Naprendszerek száma
    public int starCount;

    //Nézet
    public static bool GalaxyView;
    public bool fromGalaxy;

    //
    bool GenerationComplete;

    #endregion

    void Awake()
    {
        #region Első lefutás
        if (ParameterWatcher.firstGameInit)
        {
            #region Naprendszerek száma beállítás
            if (GameStartOptions.StarCount != 0)
            {
                starCount = GameStartOptions.StarCount;
            }
            else
            {
                starCount = 5;
            }
            #endregion

            GenerateSolarSystems(starCount); // Naprendszer generálás
            GenerateSystemRelations();       // Naprendszer kapcsolatok generálása
            initTechTree();                  // Tech fa létrehozása a Playernek

            Game.mainCamera = Camera.main;   //kezdő kamera beállítása
            Game.game = this;                //game = this

            #region Playerek deklarálása

            players = new List<Player>();

            currentPlayer = new Player(10000, 10000, 10000, "Peti", Species.Human);
            player2 = new Player(10000, 10000, 10000, "Sanyi", Species.Human);
            player2.MaxPopulation = 200;

            currentPlayer.enemies.Add(player2);
            player2.enemies.Add(currentPlayer);

            players.Add(currentPlayer);
            players.Add(player2);

            #endregion

            //Kezdő naprendszer
            startSolarSystem = Systems[0];
            currentSolarSystem = startSolarSystem;

            //Kezdő nézet beállítása
            fromGalaxy = false;

            ParameterWatcher.firstGameInit = false;
        }
        #endregion
    }

    void Start()
    {
        #region Alap egységek/épületek legenerálása

        GenerationComplete = false;
        for (int i = 0; i < players.Count; i++)
        {
            GameObject mainBuilding = Instantiate(players[i].BuildableUnits.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().isDropOffPoint).First());
            mainBuilding.GetComponent<Structure>().Owner = players[i].empireName;
            mainBuilding.name = players[i].BuildableUnits.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().isDropOffPoint).First().name;
            mainBuilding.SetActive(true);
            mainBuilding.transform.SetParent(solarSystemPrefabs[i].transform.Find("Units"));
            mainBuilding.transform.position = GetStartingPosition(i);
            players[i].units.Add(mainBuilding);

            GameObject mainBarracks = Instantiate(players[i].BuildableUnits.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().TrainableUnits.Count>1).First());
            mainBarracks.GetComponent<Structure>().Owner = players[i].empireName;
            mainBarracks.name = players[i].BuildableUnits.Where(x => x.GetComponent<Structure>() && x.GetComponent<Structure>().TrainableUnits.Count>1).First().name;
            mainBarracks.SetActive(true);
            mainBarracks.transform.SetParent(solarSystemPrefabs[i].transform.Find("Units"));
            mainBarracks.transform.position = mainBuilding.transform.position + new Vector3(25, 0, 25);
            players[i].units.Add(mainBarracks);

            for (int j = 0; j < 5; j++)
            {
                GameObject gatherer = Instantiate(players[i].BuildableUnits.Where(x => x.GetComponent<Unit>() && x.GetComponent<Unit>().isGatherer).First());
                gatherer.GetComponent<Unit>().Owner = players[i].empireName;
                gatherer.name = players[i].BuildableUnits.Where(x => x.GetComponent<Unit>() && x.GetComponent<Unit>().isGatherer).First().name;
                gatherer.SetActive(true);
                gatherer.transform.SetParent(solarSystemPrefabs[i].transform.Find("Units"));
                gatherer.transform.position = mainBuilding.transform.position + new Vector3(j * 4, -5, 5);
                players[i].units.Add(gatherer);
            }
        }
        #endregion
    }

    Vector3 GetStartingPosition(int i)
    {
        Vector3 startPosition = new Vector3();

        if (solarSystemPrefabs[i].transform.Find("Planets").Find("Iridium Asteroid") != null)
            startPosition = solarSystemPrefabs[i].transform.Find("Planets").Find("Iridium Asteroid").transform.position;
        if (solarSystemPrefabs[i].transform.Find("Planets").Find("Palladium Asteroid") != null)
            startPosition = solarSystemPrefabs[i].transform.Find("Planets").Find("Palladium Asteroid").transform.position;
        else startPosition = solarSystemPrefabs[i].transform.Find("Planets").Find("Element Zero Asteroid").transform.position;

        startPosition.x += 50;
        startPosition.y = 0;
        startPosition.z -= 30;

        return startPosition;
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
                player.units[i].GetComponent<Unit>().Armor += player.units[i].GetComponent<Unit>().ArmorIncrement;
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
                player.units[i].GetComponent<Unit>().attackDamage += player.units[i].GetComponent<Unit>().AttackIncrement;
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
