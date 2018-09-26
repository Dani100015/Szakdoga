using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Planets : Celestials
{
    bool isColonized;
    float minTemperature;
    float maxTempretature;

    //List<Structures>() buildings;


    public Planets(SolarSystem sol, string name, float x, float y) : base(sol, name, x, y)
    {
        
    }
}
