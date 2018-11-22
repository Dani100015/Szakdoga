using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class ChangeSolarSytem : MonoBehaviour {

    //Script elemek
    Game game;
    SetSolarSystems setSolarSystem;

    //Naprendszerek
    List<SolarSystem> Systems;
    List<GameObject> SystemPrefabs;

    //Listaelem
    public GameObject ItemGObject;

    //UI Text
    public Text ItemStarnameText;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        setSolarSystem  = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();

        transform.GetComponent<Button>().onClick.AddListener(() => ChangeSystem());

        Systems = game.Systems;
        SystemPrefabs = SetSolarSystems.SystemGObjects;

        ItemGObject = transform.gameObject;
        ItemStarnameText = ItemGObject.transform.Find("ItemText").GetComponent<Text>();
    }
    /// <summary>
    /// A listaelem álatal megadott naprendszerre vált
    /// </summary>
    public void ChangeSystem()
    {
        setSolarSystem.DisappearOtherSolarSystem(SystemPrefabs.Find(x => x.name == ItemStarnameText.text));
        setSolarSystem  = GameObject.Find("SolarSystemGenerator").GetComponent<SetSolarSystems>();
        game.currentSolarSystem = game.Systems.Find(x => x.Name == ItemStarnameText.text);
    }
}
