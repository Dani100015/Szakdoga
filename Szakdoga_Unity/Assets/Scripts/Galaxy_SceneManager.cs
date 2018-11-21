using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Galaxy_SceneManager : MonoBehaviour
{

    Game game;
    GameObject selectedObject;
    RaycastHit hitInfo;
    bool hit;
    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();

    }
    void Update()
    {
        #region UnitTravelOrder
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            hitInfo = new RaycastHit();
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

                                    CurrentObject.GetComponent<Unit>().ActionsQueue.Clear();
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region SolarSystem View Change
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            hitInfo = new RaycastHit();
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

                    game.currentSolarSystem = game.Systems.Find(x => x.Name == selectedObject.name);
                    Game.GalaxyView = false;

                    CameraViewChange.ChangeCameraView();
                }
            }
        }
        #endregion
    }
}
