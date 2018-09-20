using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tech
{

    //alap adatok
    string name;
    string description;
    Tech prerequisite;
    Tech leadsTo;
    int researchTime;
    bool researched;

    //nyersanyag szükséglet
    int palladiumCost;
    int iridiumCost;
    int eezoCost;

    #region Propertyk 
    public bool Researched
    {
        get; set;
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public Tech Prerequisite
    {
        get { return prerequisite; }
        set { prerequisite = value; }
    }

    public Tech LeadsTo
    {
        get { return leadsTo; }
        set { leadsTo = value; }
    }

    public int ResearchTime
    {
        get { return researchTime; }
        set { researchTime = value; }
    }

    public int PalladiumCost
    {
        get { return palladiumCost; }
        set { palladiumCost = value; }
    }

    public int IridiumCost
    {
        get { return iridiumCost; }
        set { iridiumCost = value; }
    }

    public int EezoCost
    {
        get { return eezoCost; }
        set { eezoCost = value; }
    }
    #endregion

    public override string ToString()
    {
        return this.Name;
    }
}
