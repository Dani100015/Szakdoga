using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class SolarSystem
{

    string name;
    Player owner;                                   //Naprendszer irányítója
    public List<Celestials> celestials;            //Naprendszer égitestjei
    public List<SolarSystem> neighbourSystems;     //Szomszédos naprendszerek
    public GameObject solarSystemGObject;

    public Vector3 position;

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

    public SolarSystem(Vector3 position,string name, Player owner, List<Celestials> celestials)
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

    static public Vector3 DistanceBetweenStars(Vector3 pos1, Vector3 pos2)
    {
        return new Vector3( pos1.x - pos2.x, 
                            pos1.y - pos2.y, 
                            pos1.z - pos2.z);


    }


}