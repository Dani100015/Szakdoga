﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class ChangeSolarSytem : MonoBehaviour {

    Game game;
    SetSolarSystems setSolarSystem;

    List<SolarSystem> solarSystems;
    List<GameObject> solarSystemPrefabs;

    public GameObject ItemGObject;
    public Text ItemStarnameText;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        setSolarSystem  = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();

        transform.GetComponent<Button>().onClick.AddListener(() => ChangeSystem());

        solarSystems = game.Systems;
        solarSystemPrefabs = setSolarSystem.SystemPrefabs;

        ItemGObject = transform.gameObject;
        ItemStarnameText = ItemGObject.transform.Find("ItemText").GetComponent<Text>();
    }
    public void ChangeSystem()
    {

        setSolarSystem.DisappiareOtherSolarSystem(solarSystemPrefabs.Find(x => x.name == ItemStarnameText.text));
        setSolarSystem  = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();
        game.currentSolarSystem = game.Systems.Find(x => x.Name == ItemStarnameText.text);
    }
    void Update()
    {


    }
}
