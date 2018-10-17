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
    

    public Player(int Palladium = 500, int Irididium = 500, int NullElement = 100)
    {
        palladium = Palladium;
        iridium = Irididium;
        nullElement = NullElement;
    }

    public int Palladium
    {
        get
        {
            return palladium;
        }
        set
        {
            palladium = value;
        }
    }
    public int Iridium
    {
        get
        {
            return iridium;
        }
        set
        {
            iridium = value;
        }
    }
    public int NullElement
    {
        get
        {
            return nullElement;
        }
        set
        {
            nullElement = value;
        }
    }
}