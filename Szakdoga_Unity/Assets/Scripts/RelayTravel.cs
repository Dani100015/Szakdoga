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
    SolarSystem currentSystem;

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

    Vector3 activePosition;
    Vector3 deactivePosition;



    void Awake()
    {
        itemView = GameObject.Find("RelayTravelPanel").GetComponent<ScrollRect>();
        itemContents = GameObject.Find("RelayListContent");
        itemPrefab = (GameObject)Resources.Load("Prefabs/GUI/Item1", typeof(GameObject));

        game = GameObject.Find("Game").GetComponent<Game>();
        setSystem = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();
    }
    void Start () {

        

        items = new List<GameObject>();
        changes = new List<ChangeSolarSytem>();

        systemGObject = transform.parent.gameObject;
        Systems = game.Systems;

        currentSystem = Systems.Find(x => x.Name == game.currentSolarSystem.Name);


        activePosition = new Vector3(150,400,0f);
        deactivePosition = itemView.transform.position;
    }

    void GenerateNeighbourSystemList(SolarSystem system)
    {
        for (int i = 0; i < system.neighbourSystems.Count; i++)
        {
            GameObject scrollItemObject = Instantiate(itemPrefab);
            items.Add(scrollItemObject);
            scrollItemObject.AddComponent<ChangeSolarSytem>();
            scrollItemObject.transform.SetParent(itemContents.transform, false);


            scrollItemObject.name = "Item" + i;
            scrollItemObject.AddComponent<Button>();
            

            solarSystemText = scrollItemObject.transform.Find("ItemText").GetComponent<Text>();
            solarSystemText.text = system.neighbourSystems[i].Name;

        }

    }
    void Update()
    {
        
        if (currentSystem.Name != game.currentSolarSystem.Name)
        {
            isRelayPanelActive = !isRelayPanelActive;
            itemView.transform.position = deactivePosition;
            for (int i = 0; i < itemContents.transform.childCount; i++)
            {               
                Destroy(itemContents.transform.GetChild(i).gameObject);            
            }

            currentSystem = game.currentSolarSystem;
        }
    }


    void OnMouseDown()
    {
        currentSystem = game.currentSolarSystem;

        if (isRelayPanelActive == true)
        {
            itemView.transform.position = activePosition;
            GenerateNeighbourSystemList(currentSystem);
        }
        else if (isRelayPanelActive == false)
        {
            itemView.transform.position = deactivePosition;
            for (int i = 0; i < itemContents.transform.childCount; i++)
            {
                Destroy(itemContents.transform.GetChild(i).gameObject);
            }
        }
        isRelayPanelActive = !isRelayPanelActive;

    }

}
