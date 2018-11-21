using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Galaxy_SceneManager : MonoBehaviour
{
    //Scriptek
    Game game;

    static GameObject selectedObject; //Kiválaszott objectum
    static RaycastHit hitInfo;        //Raycast Találat infó
    static bool hit;                  //Volt találat?
    static bool wasStarHit = false;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
    }
    void Update()
    {

        hitInfo = new RaycastHit();
        if (Input.GetKey(KeyCode.Mouse1))
        {
            UnitTravelOrder();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            SystemViewChange();
        }
        else if (Input.GetKey(KeyCode.Space) && game.fromGalaxy == true)
        {
            CameraViewChange.ChangeCameraView();
        }
    }

    void UnitTravelOrder()
    {
        hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hit)
        {
            if (selectedObject != hitInfo.transform.gameObject)
            {
                selectedObject = hitInfo.transform.gameObject;
            }
            else if (selectedObject.tag == "StarSystem")
            {
                selectedObject = hitInfo.transform.gameObject;
                if (Mouse.CurrentlySelectedUnits.Count != 0)
                {
                    for (int i = 0; i < Mouse.CurrentlySelectedUnits.Count; i++)
                    {
                        GameObject CurrentObject = Mouse.CurrentlySelectedUnits[i] as GameObject;
                        if (CurrentObject != null)
                        {
                            AIDestinationSetter setter = CurrentObject.GetComponent<AIDestinationSetter>();
                            Unit unitObj = CurrentObject.GetComponent<Unit>();
                            if (unitObj != null && CurrentObject.GetComponent<Structure>() == null && unitObj.Owner == Game.currentPlayer.empireName)
                            {
                                Transform unitOwnSolarSystem = CurrentObject.transform.parent.parent;

                                Debug.Log(unitOwnSolarSystem.gameObject.name);

                                setter.target = GameObject.Find("SolarSystems").transform.Find(unitOwnSolarSystem.name).transform.Find("BUILDING_Relay");
                                unitObj.solarSystemTarget = selectedObject;
                                Debug.Log(unitObj.solarSystemTarget.name);

                                CurrentObject.GetComponent<Unit>().ActionsQueue.Clear();
                            }
                        }
                    }
                }
            }
        }

    }
    void SystemViewChange()
    {
        hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hit)
        {
            if (selectedObject != hitInfo.transform.gameObject)
            {
                selectedObject = hitInfo.transform.gameObject;
            }
            else if (selectedObject.tag == "StarSystem" )
            {
                selectedObject = hitInfo.transform.gameObject;
                Game.GalaxyView = false;
                game.currentSolarSystem = game.Systems.Find(x => x.Name == selectedObject.name);
                CameraViewChange.ChangeCameraView();               
            }
        }
        
    }
}
