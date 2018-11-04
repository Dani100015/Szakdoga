using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class GUI_StarNamePanel : MonoBehaviour {

    public Canvas canvas;
    public TextMeshProUGUI planetNameText;
    string planetName;

	void Start () { 

        planetName = transform.parent.gameObject.name;
        planetNameText.text = planetName;

	}
	
}
