using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class RelayTravel : MonoBehaviour {

    //Scriptek
    Game game;
    SetSolarSystems setSystem;

    //Naprendszer listák
    List<SolarSystem> neighbourSystems = new List<SolarSystem>();
    List<SolarSystem> Systems;
    List<GameObject> SystemPrefabs;

    List<ChangeSolarSytem> changes;
    List<GameObject> items;

    GameObject systemPrefab;
    SolarSystem currentSystem;
   
    //UI Panel elemek
    ScrollRect itemView;
    GameObject itemContents;
    GameObject itemPrefab;
    
    //UI Text
    Text solarSystemText; 

    //Panel pozíciók
    Vector3 activePosition;
    Vector3 deactivePosition;

    //Panel láthatósághoz
    bool isRelayPanelActive = true;

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

        systemPrefab = transform.parent.gameObject;
        Systems = game.Systems;

        currentSystem = Systems.Find(x => x.Name == game.currentSolarSystem.Name);

        activePosition = new Vector3(150,400,0f);
        deactivePosition = itemView.transform.position;
    }

    /// <summary>
    /// A RelayTravel panelbe generáljuk bele a jelenelgi naprendszer szomszédjainak a listájából listelelemek
    /// </summary>
    /// <param name="system"></param>
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

        //Megvizsgáljuk hogy aktív-e a RelayTravel panel
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
