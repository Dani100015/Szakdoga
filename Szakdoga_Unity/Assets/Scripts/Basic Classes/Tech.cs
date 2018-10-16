using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tech
{
    //alap adatok
    string name;
    string description;
    Tech prerequisite;
    int researchTime;
    bool researched;

    //nyersanyag szükséglet
   public int palladiumCost;
   public int iridiumCost;
   public int eezoCost;

    public Tech(string name, string desc, Tech pre, int resTime, int pCost, int iCost, int eCost)
    {
        this.Name = name;
        this.Description = description;
        this.Prerequisite = pre;
        this.researched = false;
        this.ResearchTime = resTime;
        this.PalladiumCost = pCost;
        this.IridiumCost = iCost;
        this.EezoCost = eCost;
    }

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
