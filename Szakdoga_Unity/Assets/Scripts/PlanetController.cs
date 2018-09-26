using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetController : MonoBehaviour {


    public string planetName;
    Player owner;
    SolarSystem system;

    Planets terra;
	
    void Start () {
        terra = new Planets(system, planetName, transform.position.x, transform.position.y);
	}

    GameObject selectedObject;
	void Update ()
    {
        SelectObject();

        if (selectedObject != null)
        {
            Debug.Log(terra.name + ", x:" + terra.x + ", y:" + terra.y);
        }
    }
    void SelectObject()
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
                else
                {
                    SceneManager.LoadScene(selectedObject.name.ToString());
                }
            }
        }
    }
    void LogInfo()
    {
        if (selectedObject != null)
        {

        }
    }
}
