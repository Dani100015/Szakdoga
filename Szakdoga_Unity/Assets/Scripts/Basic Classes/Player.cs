using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    public string empireName;
    ArrayList structures;
    Species species;

    List<SolarSystem> playerSystems; //irányított naprendszerek
    List<Planets> playerColonizedPlanets; //kolonizált player planéták

    List<Unit> units; //
    List<Player> allies;
    List<Player> enemies;

    public int palladium;      //
    public int iridium;        //Nyersanyagok
    public int nullElement;    //

    int Population;       //Egységek
    int maxPopulation;    //Max egységek

    //List<Unit>
    

    public Player(string name)
    {
        this.empireName = name;
    }
}