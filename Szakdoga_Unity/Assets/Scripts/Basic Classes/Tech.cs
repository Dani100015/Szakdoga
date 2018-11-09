using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tech
{
    //alap adatok
    public string name;
    string Displayname;
    string description;
    Tech prerequisite;
    int researchTime;
    bool researched;
    int techLevel; //player ben kezelni le

    static public List<Tech> techList = new List<Tech>();

   //nyersanyag szükséglet
   public int palladiumCost;
   public int iridiumCost;
   public int eezoCost;

    public Tech(string name, string desc,int resTime, int pCost, int iCost, int eCost, Tech prerequisite)
    {
        this.Name = name;
        this.Description = desc;
        this.prerequisite = prerequisite;
        this.researched = false;
        this.ResearchTime = resTime;
        this.PalladiumCost = pCost;
        this.IridiumCost = iCost;
        this.EezoCost = eCost;

        techList.Add(this);
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

    #region TechInfos

    static string ferroUraniumSlogs_Info = "Use of significantly harder materials to create the ammunition blocks of mass accelertor weapons result in a slug that retains its shape much longer upon impact. This improvement increases the damage dealt by all mass accelerator weapon.";
    static string efficientRailTracks_Info = "Volus efficiency specialists apply their knowledge to the desing of mass accelerator weapon.Resultant improvement to the rail elemnts of the weapon allow a higher velocity slug to be fired, increasing the damage of all mass acceleator weapons.";
    static string lowerProjectiveMass_Info = "Mass effect field used in accelerators are enhanced by Salarian engineers to further lower the mass of projectiles before firing, resultuing in an even higher projectile velocity. This will further increase the damage of all mass accelerator weapons.";
    static string hawkingCarrier_Info = "The Systems Alliance eagerly grants the Coouncil fleet production rights for the smaller of their carrier designs. this ship is an efficient fighter carrying platform, but lacks severely in defense, forcing it to operte ont he edges of a battle.";
    static string yorkClassDreadnaught_Info = "A veteran Alliance York Dreadnought, the Skyang Kangri may not be the msoot formidable ship available to the Council, but it remains a powerful brawler. With significiant armor and an unrivaled broadside, the Skyang Kangri is most at home in the middle of the enemy fleet.";
    static string einsteinCarrier_Info = "Access to the heavier Alliance carrier, the Einstein class, is granted. this large carrier has the defences needed to operate in the midst of battle, as well as carrying a large number of infantry and their landing craft for securing planets in the name of the Council.";
    static string turianDesignElements_Info = "Turian bulkhead construction techniques and damage control methods are adapted for use throughout the fleet, increasing the maximum hull points and passive repair rates of all hulls.";
    static string highStressMmaterials_Info = "The Elcor provides advanved materials for ship hull contruction that are both more resilient and easier to repair than standards hull materials. this increases the maximum hull points and passive repair rate of all hulls.";
    static string quarianMiracleWorkers_Info = "Quarian engineers are seconded to ships throughout the fleet to teach crews how to achive more with less when it comes to maintences and reapir. Increases maximum hull and passive reapir rate of all hulls.";
    static string kroganArmorDesign_Info = ": Though Krogan scientists are largely scoffed at by tha galactic community, armor is one area in which they have consistenly excelled throughout history. By adapting Krogan armor construction techniques to starships, Council fleet survivability can be significantly increased.";
    static string silarisDistribution_Info = "The Asasi release production specifications for their advanced Silaris armor to the rest of the Concuil forces. With this advanced armor being applied to critical sections or all ships, Council fleet survivability is significcantly increased.";
    static string rapidHarvesting_Info = "Quarian miners are some of the fastest and most efficient in the Council. Drawing from their experiance rapidly strip mining planetoids of Eezo to keep the migrant fleet fueld, these Quarian experts can improve the overall speed of mining for the Council.";
    static string qualityControl_Info = "The Asari have a far greater understanding of Eezo than many others due to their world being naturally rich in it. Asari mining engineers are now employed throughout Council space to streamline the harvesting the overall speed of harvesting.";
    static string ancientMethods_Info = "From varying sources, information is collected on how the Prothean Empire once harvested Eezo. Applying some of these ancient methods of harvesting in modern mines further improves  the Council ability to quickly harvest Eezo.";
    static string turianMilitaryCities_Info = "Cities on Desert worlds are militrarised in classic turian style, encouraging greater number of Turians to settle on such worlds.";
    static string kroganHardiness_Info = "Korgan settlers are capable of setting up colonies almost anywhere. With Council support, Desert worlds receive additional Krogan settlers who set up towns in even the harshest of the world’s environments.";
    static string asarMegacities_Info = "Expert Asari civic planners are emplyed by city leadership on Garden worlds, leading to more efficient cities with greater populations.";
    static string cSecConscription_Info = "Local militia have been conscripted to C-Sec to bolster their unit numbers.";
    static string kroganMercenaries_Info = "Krogan units have been drafted into the fllet, their combat experiance make them valueable warriors.";
    static string allianceDefenseBoard_Info = "Alliance military officials agree to deploy more forces to assist the Citadel fleet.";
    static string asariHighCommand_Info = "Commando units will be deployed throughout the fleet.";
    static string citadelDefenceBoard_Info = "The combined forces of the Citadel races have ceded larger portions of their command to the Council Defence Committee.";

    #endregion

    #region Technológiák
    //Weapon
    public static Tech FerroUraniumSlogs = new Tech("FerroUraniumSlogs", ferroUraniumSlogs_Info, 10, 100, 100, 100, null);
    public static Tech EfficientRailTracks = new Tech("EfficientRailTracks", efficientRailTracks_Info, 10, 100, 100, 100, FerroUraniumSlogs);
    public static Tech LowerProjectiveMass = new Tech("LowerProjectiveMass", lowerProjectiveMass_Info, 10, 100, 100, 100, EfficientRailTracks);
    //Ships
    public static Tech HawkingCarrier = new Tech("HawkingCarrier", hawkingCarrier_Info, 10, 100, 100, 100, null);
    public static Tech YorkDreadnought = new Tech("YorkDreadnought", yorkClassDreadnaught_Info, 10, 100, 100, 100, null);
    public static Tech EinsteinCarrier = new Tech("EinsteinCarrier", einsteinCarrier_Info, 10, 100, 100, 100, null);
    //Health
    public static Tech TurianDesignElements = new Tech("TurianDesignElements", turianDesignElements_Info, 10, 100, 100, 100, null);
    public static Tech HighStressMaterials = new Tech("HighStressMaterials", highStressMmaterials_Info, 10, 100, 100, 100, TurianDesignElements);
    public static Tech QuarianMiracleWorkers = new Tech("QuarianMiracleWorkers", quarianMiracleWorkers_Info, 10, 100, 100, 100, HighStressMaterials);
    //Armor
    public static Tech KroganArmorDesign = new Tech("KroganArmorDesign", kroganArmorDesign_Info, 10, 100, 100, 100, null);
    public static Tech SilarisDistribution = new Tech("SilarisDistribution", silarisDistribution_Info, 10, 100, 100, 100, KroganArmorDesign);
    //Harvest
    public static Tech RapidHarvesting = new Tech("RapidHarvesting", turianDesignElements_Info, 10, 100, 100, 100, null);
    public static Tech QualityControl = new Tech("QualityControl", highStressMmaterials_Info, 10, 100, 100, 100, RapidHarvesting);
    public static Tech AncientMethods = new Tech("AncientMethods", quarianMiracleWorkers_Info, 10, 100, 100, 100, QualityControl);
    //Fleet
    public static Tech CSecConscription = new Tech("CSecConscription", turianDesignElements_Info, 10, 100, 100, 100, null);
    public static Tech KroganMercenaries = new Tech("KroganMercenaries", highStressMmaterials_Info, 10, 100, 100, 100, CSecConscription);
    public static Tech AllianceDefenseBoard = new Tech("AllianceDefenseBoard", quarianMiracleWorkers_Info, 10, 100, 100, 100, KroganMercenaries);
    public static Tech AsariHighCommand = new Tech("AsariHighCommand", kroganArmorDesign_Info, 10, 100, 100, 100, AllianceDefenseBoard);
    public static Tech CitadelDefenceBoard = new Tech("CitadelDefenceBoard", silarisDistribution_Info, 10, 100, 100, 100, AsariHighCommand);
    #endregion


}


