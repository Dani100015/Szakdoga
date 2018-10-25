using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGhost : MonoBehaviour {	
	void Update () {
        //Ghost egység helyzetének frissítése
        transform.position = new Vector3(Mouse.currentMousePoint.x, 5f, Mouse.currentMousePoint.z);

        //Jobb kattintásra visszavonja a Ghost egységet
        if (Input.GetMouseButtonUp(1))
        {
            Destroy(this.gameObject);
            GUISetup.GhostActive = false;
        }
	}
}
