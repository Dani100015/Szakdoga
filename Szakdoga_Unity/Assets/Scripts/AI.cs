using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour {

    public Player player;
    public ArrayList CurrentlySelectedUnits;
    public ArrayList CurrentWorkers;
    public ArrayList PalladiumGatherers;
    public ArrayList IridiumGatherers;
    public ArrayList EezoGatherers;

    public ArrayList AttackForce;
    public ArrayList DefenceForce;
    public GameObject MainBuilding;

	// Use this for initialization
	void Start () {
        player = Game.player2;
        CurrentlySelectedUnits = new ArrayList();
        CurrentWorkers = new ArrayList();
        PalladiumGatherers = new ArrayList();
        IridiumGatherers = new ArrayList();
        EezoGatherers = new ArrayList();
        AttackForce = new ArrayList();
        DefenceForce = new ArrayList();    
    }
	
	// Update is called once per frame
	void Update () {
        if (player == null)
            player = Game.player2;
        if (MainBuilding == null)
            MainBuilding = player.units.Where(x => x.GetComponent<Structure>() != null && x.GetComponent<Structure>().isDropOffPoint).FirstOrDefault();
        Debug.Log(player.units.Count);
        #region Gyűjtögetők
        #endregion
    }
}
