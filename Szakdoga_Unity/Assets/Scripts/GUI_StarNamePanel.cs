using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class GUI_StarNamePanel : MonoBehaviour {

    Canvas canvas;
    TextMeshProUGUI planetNameText;
    string planetName;

	void Start () {

       
        canvas = transform.GetComponent<Canvas>();
        planetNameText = transform.Find("Panel").transform.Find("PlanetText").GetComponent<TextMeshProUGUI>();


        planetName = canvas.gameObject.transform.parent.name;       
        planetNameText.text = planetName;

	}
	
}
