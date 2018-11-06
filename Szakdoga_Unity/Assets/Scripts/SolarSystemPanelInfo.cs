using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class SolarSystemPanelInfo : MonoBehaviour
{

    GameObject[] solarSystems;
    Text SolarSystemName;

    SetSolarSystems setSystems;
    Game game;

    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        setSystems = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();

        // Debug.Log("SolarSystemPanelInfo:")

        SolarSystemName = transform.Find("TextSolarSystem").GetComponent<Text>();
    }
    void Update()
    {
        InvokeRepeating("UpdatetSolarSystemText", 1, 2);
    }
    void UpdatetSolarSystemText()
    {
        if (setSystems != null && setSystems.currentSystemPrefab != null)
        {
            SolarSystemName.text = setSystems.currentSystemPrefab.name;
        }

    }

}
