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

    int palladium;      //
    int iridium;        //Nyersanyagok
    int nullElement;    //

    int Population;       //Egységek
    int maxPopulation;    //Max egységek

    //List<Unit>
    

    public Player(string name)
    {
        this.empireName = name;
    }
}