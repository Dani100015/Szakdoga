﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class Game : MonoBehaviour
{

    public static List<Player> players;
    //List<Team> teams;

    //public Game origGame;
    //public Game currentGame; 

    public static Player currentPlayer;
    public static Player player2;

    public static object[] SharedIcons;

    public List<SolarSystem> Systems;

    public List<GameObject> galaxyStarPrefabs;
    public List<GameObject> solarSystemPrefabs;

    public SolarSystem startSolarSystem;
    public SolarSystem currentSolarSystem;

    public int starCount;




    void Awake()
    {
        starCount = 5;

        GenerateSolarSystems(starCount);
        GenerateSystemRelations();

        startSolarSystem = Systems[0];
        currentSolarSystem = startSolarSystem;

        //if (GameObject.FindGameObjectsWithTag("Game").Length >= 2)
        //{
        //    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Game").Length; i++)
        //    {
        //        if (GameObject.FindGameObjectsWithTag("Game")[i].scene.name == "SolarSystem")
        //        {
        //            origGame = GameObject.FindGameObjectsWithTag("Game")[i].GetComponent<Game>();
        //        }
        //        else
        //        {
        //            currentGame = GameObject.FindGameObjectsWithTag("Game")[i].GetComponent<Game>();
        //        }
        //    }
        //}

    }
    void Start()
    {

        #region Init
        //Játékosok inicializálása
        //players = new List<Player>();

        currentPlayer = new Player(100, 100, 100, "Peti");
        player2 = new Player(100, 100, 100, "Sanyi");

        //players.Add(currentPlayer);
        //players.Add(player2);

        //Egységek betöltése és játékosokhoz rendelése
        string path = "Prefabs/Units";
        object[] Units = Resources.LoadAll(path);
        if (Units.Length > 0)
        {
            for (int i = 0; i < Units.Length; i++)
            {
                GameObject unit = Units[i] as GameObject;
                Texture2D unitIcon = unit.GetComponent<Unit>().MenuIcon;
                Texture2D unitIconRo = unit.GetComponent<Unit>().MenuIconRo;

                //foreach (Player p in players)
                //{
                //    p.UnitIcons.Add(unitIcon);
                //    p.UnitIconsRo.Add(unitIconRo);
                //    p.UnitNames.Add(unit.name);
                //    p.UnitPaths.Add(path + "/" + unit.name);
                //}
            }
        }

        SharedIcons = Resources.LoadAll("Icons/Shared");

        #endregion

    }

    void Update()
    {

    }

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

                    if (!Systems[i].neighbourSystems.Contains(Systems[rndIndex]) && Systems[rndIndex].Name != Systems[i].Name )
                    {
                        Systems[i].neighbourSystems.Add(Systems[rndIndex]);
                    }
                }


            }
        }
    }
}
   

