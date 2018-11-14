using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Galaxy_SceneManager : MonoBehaviour {

    Game game;
    GameObject selectedObject;


    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {

                if (selectedObject != hitInfo.transform.gameObject)
                {
                    selectedObject = hitInfo.transform.gameObject;
                }
                else if(selectedObject.tag == "StarSystem")
                {
                    selectedObject = hitInfo.transform.gameObject;

                    game.currentSolarSystem = game.Systems.Find(x => x.Name == selectedObject.name);
                    game.fromGalaxy = true; 
                 
                    SceneManager.LoadScene("SolarSystems");                                   
                }
            }
        }
    }
}
