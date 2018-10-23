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

    //Egységek importálása
    public List<Texture2D> UnitIcons = new List<Texture2D>();
    public List<Texture2D> UnitIconsRo = new List<Texture2D>();
    public List<string> UnitNames = new List<string>();
    public List<string> UnitPaths = new List<string>();

    public Player(int pall, int irid, int eezo, string name)
    {
        palladium = pall;
        iridium = irid;
        nullElement = eezo;
        empireName = name;
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