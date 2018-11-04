using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    List<SolarSystem> solars;
    public static List<Player> players;
    //List<Team> teams;

    public static Player currentPlayer;
    public static Player player2;

    public static object[] SharedIcons;

    public GameObject starPrefab;
    public GameObject planetPrefab;

    SolarSystem solarSystem1;
    SolarSystem solarSystem2;

    void Start () {

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
        solarSystem1 = new SolarSystem("solarSystem1", currentPlayer, null,null);
        solarSystem1.InitCelestials();

        InitSolarSystem();

    }
	
	void Update () {
		
	}

    // void CheckForWin
    // void CheckForLose
    // void setNewGame
    // void EndGame


    void InitSolarSystem()
    {
        //GameObject star = (GameObject)Instantiate(Resources.Load("Assets/_Prefabs/Galaxy/Star.prefab"));
        GameObject star = Instantiate(starPrefab);
        star.transform.position = new Vector3(0, 0, 0);

        GameObject planet;
        for (int i = 0; i < solarSystem1.celestials.Count; i++)
        {
            planet = Instantiate(planetPrefab);
            planet.transform.position = new Vector3(solarSystem1.celestials[i].x, 0, solarSystem1.celestials[i].y);
        }

    }

    public static void ResearchEffects(Player player, Tech tech)
    {

    }
}
