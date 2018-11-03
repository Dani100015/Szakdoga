using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class RelayTravel : MonoBehaviour {

    Game game;
    SetSolarSystems setSystem;
    List<ChangeSolarSytem> changes;

    GameObject systemGObject;
    SolarSystem system;

    Component[] meshComponents;

    List<SolarSystem> neighbourSystems = new List<SolarSystem>();
    List<SolarSystem> Systems;
    List<GameObject> SystemGObjects;

    GameObject currentSolarSystemGObject;

    public ScrollRect itemView;
    public GameObject itemContents;
    public GameObject itemPrefab;

    List<GameObject> items;

    Text solarSystemText;

    bool isRelayPanelActive = true;

    void Awake()
    {
        itemView = GameObject.Find("RelayTravelSystems").GetComponent<ScrollRect>();
        itemContents = GameObject.Find("RelayListContent");
        itemPrefab = (GameObject)Resources.Load("Prefabs/GUI/Item1", typeof(GameObject));
    }
    void Start () {

        game = GameObject.Find("Game").GetComponent<Game>();
        setSystem = GameObject.Find("Game").GetComponent<SetSolarSystems>();

        items = new List<GameObject>();
        changes = new List<ChangeSolarSytem>();

        systemGObject = transform.parent.gameObject;
        Systems = game.Systems;
        system = Systems.Find(x => x.Name == systemGObject.name);

        currentSolarSystemGObject = game.solarSystemPrefabs.Find(x => x.name == game.currentSolarSystem.Name);

        neighbourSystems = system.neighbourSystems;
        itemView.enabled = false;

        
    }

    void GenerateNeighbourSystemList(int starCount)
    {
        for (int i = 0; i < starCount; i++)
        {
            GameObject scrollItemObject = Instantiate(itemPrefab);
            items.Add(scrollItemObject);
            scrollItemObject.AddComponent<ChangeSolarSytem>();
            scrollItemObject.transform.SetParent(itemContents.transform, false);           

            scrollItemObject.name = "Item" + i;
            scrollItemObject.AddComponent<Button>();
            
            solarSystemText = scrollItemObject.transform.Find("ItemText").GetComponent<Text>();
            solarSystemText.text = neighbourSystems[i].Name;

        }

    }

    void OnMouseDown()
    {
        if (isRelayPanelActive == true)
        {
            itemView.gameObject.SetActive(true);         
            GenerateNeighbourSystemList(neighbourSystems.Count);
        }
        else if (isRelayPanelActive == false)
        {
            itemView.gameObject.SetActive(false);
            for (int i = 0; i < itemContents.transform.childCount; i++)
            {
                Destroy(itemContents.transform.GetChild(i).gameObject);
            }
        }

        isRelayPanelActive = !isRelayPanelActive;

    }

}
