using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Game : MonoBehaviour {

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

    public bool fromGalaxy;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        setSystems = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();


        if (ParameterWatcher.firstInit)
        {

            //gameStart = GameObject.Find("GameStartOptions").GetComponent<GameStartOptions>();
            //starCount = gameStart.StarCount;

            starCount = 5;

            fromGalaxy = false;

            GenerateSolarSystems(starCount);
            GenerateSystemRelations();

            startSolarSystem = Systems[0];
            currentSolarSystem = startSolarSystem;

            initTechTree();

            ParameterWatcher.firstInit = false;

        }
    }

    void Start () {

        #region Init

        //Játékosok inicializálása
        players = new List<Player>();

        currentPlayer = new Player(10000, 10000, 10000, "Peti",Species.Human);
        player2 = new Player(10000, 10000, 10000, "Sanyi",Species.Reaper);

        currentPlayer.enemies.Add(player2);
        player2.enemies.Add(currentPlayer);

        players.Add(currentPlayer);
        players.Add(player2);

        //Egységek betöltése és játékosokhoz rendelése
        string path = "Prefabs/Units";
        object[] Units = Resources.LoadAll(path);
        if (Units.Length>0)
        {
            for (int i = 0; i < Units.Length; i++)
            {
                GameObject unit = Units[i] as GameObject;
                Unit unitobj = unit.GetComponent<Unit>();
                foreach (Player p in players)
                {
                    if (unitobj.Race == p.species)
                        p.BuildableUnits.Add(unit);
                }
            }
        }

        SharedIcons = Resources.LoadAll("Icons/Shared");
        #endregion
        
    }

    void Update () {
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
