using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class Celestials {

    public float x;
    public float y;
    public string name;
    public SolarSystem solarSystem;
    public Player owner;

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
    public SolarSystem SolarSystem
    {
        get
        {
            return solarSystem;
        }
        set
        {
            solarSystem = value;
        }
    }
    public Player Owner
    {
        get { return owner; }
        set { owner = value; ; }
    }
    public Celestials(SolarSystem Sol,string Name,float X,float Y)
    {
        name = Name;
        solarSystem = Sol;
        x = X;
        y = Y;
    }
    

}
