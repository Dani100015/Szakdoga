using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    List<SolarSystem> solars;
    List<Player> players;
    //List<Team> teams;

    public static Player currentPlayer;
    public static Player player2;

    public GameObject starPrefab;
    public GameObject planetPrefab;

    SolarSystem solarSystem1;
    SolarSystem solarSystem2;

    void Start () {

        //Játékosok inicializálása
        players = new List<Player>();

        currentPlayer = new Player(0, 0, 0, "Peti");
        player2 = new Player(0, 0, 0, "Sanyi");
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
                Texture2D unitIcon = unit.GetComponent<Unit>().MenuIcon;

                foreach (Player p in players)
                {
                    p.UnitIcons.Add(unitIcon);
                    p.UnitNames.Add(unit.name);
                    p.UnitPaths.Add(path + "/" + unit.name);
                }
            }
        }

        

        solarSystem1 = new SolarSystem("solarSystem1", currentPlayer, null,null);
        solarSystem1.InitCelestials();

        InitSolarSystem();

    }

    void OnGUI()
    {
      
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
}
