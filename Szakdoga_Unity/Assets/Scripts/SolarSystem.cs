using System.Collections.Generic;
using UnityEngine;

class SolarSystem
{

    string name;
    public Vector3 position;
    Player owner;                                   //Naprendszer irányítója
    public List<Celestials> celestials;            //Naprendszer égitestjei
    public List<SolarSystem> neighbourSystems;     //Szomszédos naprendszerek
    public GameObject solarSystemGObject;



    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }
    public Player Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }

    public SolarSystem(Vector3 position, string name, Player owner, List<Celestials> celestials)
    {
        this.position = position;
        this.name = name;
        this.owner = owner;
        this.celestials = celestials;
    }
    public void InitCelestials()
    {
        int numberOfPlanet = Random.Range(1, 5);
        int distanceOfPlanets = Random.Range(50, 200);
        celestials = new List<Celestials>();

        for (int i = 0; i < numberOfPlanet; i++)
        {
           // celestials.Add(new Planets(this, string.Format("PlanetName{0}", numberOfPlanet), Random.Range(-200, 200), Random.Range(-200, 200)));
        }
    }

}