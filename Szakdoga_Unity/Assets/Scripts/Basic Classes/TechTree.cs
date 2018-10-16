using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTree
{
    #region Technológiák
   public static Tech HumanDamageUpgrade1 = new Tech("Improved Mass Drivers", "Increases the attack damage of your ships", null, 100, 175, 225, 50);
   public static Tech HumanDamageUpgrade2 = new Tech("Advanced Mass Drivers", "Further increases the attack damage of your ships", HumanDamageUpgrade1, 150, 225, 275, 100);
   public static Tech HumanMiningUpgrade = new Tech("Laser Cutters", "Increases the mining speed of your workers", null, 100, 125, 220, 35);
   public static Tech HumanNuclearTorpedoes = new Tech("Nuclear Torpedoes", "Enables your Dreadnaughts to use the heavy cannon ability", HumanDamageUpgrade2, 150, 225, 275, 100);
    #endregion

    Species species;
    List<Tech> playerTechs;

    public TechTree(Species spec)
    {
        this.species = spec;
        playerTechs = new List<Tech>();
    }

    public void TechResearched(Tech tech)
    {

    }
}
