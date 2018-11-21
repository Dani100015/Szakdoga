using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class GUI_ImperialInfoPanel_UnitDetail : MonoBehaviour {

    public GameObject unitObject;
    public Camera camera;
    public Game game;

    void Awake()
    {
        camera = Camera.main;
        game = GameObject.Find("Game").GetComponent<Game>();
        transform.GetComponent<Button>().onClick.AddListener(() => OnMouseDown());
    }

    void OnMouseDown()
    {
        if (unitObject.transform.parent.parent.name != game.currentSolarSystem.Name)
        {
            game.currentSolarSystem = game.Systems.Find(x => x.Name == unitObject.transform.parent.parent.name);
        }
        camera.transform.position = unitObject.transform.position + new Vector3(0,5,-20);
    }
}
