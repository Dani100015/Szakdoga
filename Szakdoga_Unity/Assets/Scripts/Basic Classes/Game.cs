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

    void Awake()
    {
        starCount = 5;

        GenerateSolarSystems(starCount);
        GenerateSystemRelations();

        startSolarSystem = Systems[0];
        currentSolarSystem = startSolarSystem;

        initTechTree();
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

        GenerateSolarSystems(starCount);
        GenerateSystemRelations();

        
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

        //for (int i = 0; i < starCount; i++)
        //{
        //    for (int j = 0; j < starCount - 1; j++)
        //    {
        //        SolarSystem sol1 = Systems[i];
        //        SolarSystem sol2 = Systems[j];

        //        Vector3 distance = SolarSystem.DistanceBetweenStars(sol1.position, sol2.position);
        //        while (distance.x <= minDistance.x || distance.y <= minDistance.y || distance.z <= minDistance.z)
        //        {
        //            sol1.position = new Vector3((float)Random.Range(10, 500), (float)13, (float)Random.Range(10, 500));
        //        }
        //    }

        //}
    }

    void GenerateSystemRelations()
    {
        //Szomszéd viszonyság generálás
        for (int i = 0; i < Systems.Count; i++)
        {
            Systems[i].neighbourSystems = new List<SolarSystem>();

            int neigbourCount = Random.Range(1, Systems.Count);
            for (int j = 0; j < neigbourCount; j++)
            {
                while (Systems[i].neighbourSystems.Count != neigbourCount)
                {
                    int rndIndex = Random.Range(0, Systems.Count);

                    if (!Systems[i].neighbourSystems.Contains(Systems[rndIndex]) && Systems[rndIndex].Name != Systems[i].Name)
                    {
                        Systems[i].neighbourSystems.Add(Systems[rndIndex]);
                    }
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
