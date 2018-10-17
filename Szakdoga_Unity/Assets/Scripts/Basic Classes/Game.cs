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


        currentPlayer = new Player();
        player2 = new Player();

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
}
