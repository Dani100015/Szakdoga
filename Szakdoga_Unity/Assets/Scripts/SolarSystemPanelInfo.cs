﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class SolarSystemPanelInfo : MonoBehaviour
{

    GameObject[] solarSystems;
    Text SolarSystemNameText;

    SetSolarSystems setSystems;
    Game game;

    string currentSolarSystemName;

    Vector3 activePosition;
    Vector3 deactivePosition;
    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();     
        setSystems = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();

        SolarSystemNameText = transform.Find("TextSolarSystem").GetComponent<Text>();
    }

    void Start()
    {
        activePosition = new Vector3(0, 400, 0f);
        deactivePosition = new Vector3(1000,1000,0);

        transform.localPosition = activePosition;
    }
    void Update()
    {
        InvokeRepeating("UpdatetSolarSystemText", 1, 2);
        
    }
    void UpdatetSolarSystemText()
    {
        if (SceneManager.GetActiveScene().name == "Galaxy")
        {
            transform.localPosition = deactivePosition;
        }
        if (SceneManager.GetActiveScene().name == "SolarSystems")
        {
            transform.localPosition = activePosition;
            setSystems = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();
            if (setSystems != null && setSystems.currentSystemPrefab != null)
            {
                SolarSystemNameText.text = game.currentSolarSystem.Name;
            }

        }
    }
}
