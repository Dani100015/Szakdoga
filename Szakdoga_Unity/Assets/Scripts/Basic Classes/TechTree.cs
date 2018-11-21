using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTree
{

    List<Tech> techList;
    GameObject[] techItems = GameObject.FindGameObjectsWithTag("Tech");

    Species species;
    List<Tech> playerTechs;

    public TechTree(Species spec)
    {
        this.species = spec;
        playerTechs = new List<Tech>();
    }


}
