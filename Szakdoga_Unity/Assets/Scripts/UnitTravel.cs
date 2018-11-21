
using System.Collections;
using System.Collections.Generic;
using SystemPath;
using UnityEngine;

class UnitTravel : MonoBehaviour
{

    Game game;
    Unit currentUnit;
    SystemPathFinder path;
    SolarSystem targetSolarSystem;
    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        path = transform.GetComponent<SystemPathFinder>();
    }

    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.gameObject.tag == "Unit")
        {
            Debug.Log(other.GetComponent<Unit>().solarSystemTarget == null);
            if (other.GetComponent<Unit>() != null && other.GetComponent<Unit>().solarSystemTarget != null)
            {

                Debug.Log("Jeah");
                SolarSystem targetSolarSystem = game.Systems.Find(x => x.Name == other.GetComponent<Unit>().solarSystemTarget.name);
                path.FindTheWay(game.currentSolarSystem, targetSolarSystem);

                other.transform.SetParent(GameObject.Find("SolarSystems").transform.Find(other.GetComponent<Unit>().solarSystemTarget.name).transform.Find("Units"));

                if (other.GetComponent<Unit>().solarSystemTarget.name == game.currentSolarSystem.Name)
                {
                    foreach (MeshRenderer mesh in other.transform.GetComponentsInChildren<MeshRenderer>())
                    {
                        mesh.enabled = true;
                    }
                }
                else
                {
                    foreach (MeshRenderer mesh in other.transform.GetComponentsInChildren<MeshRenderer>())
                    {
                        mesh.enabled = false;
                    }

                    targetSolarSystem = game.Systems.Find(x => x.Name == other.GetComponent<Unit>().solarSystemTarget.name);
                    path.FindTheWay(game.currentSolarSystem, targetSolarSystem);

                    other.transform.SetParent(GameObject.Find("SolarSystems").transform.Find(other.GetComponent<Unit>().solarSystemTarget.name).transform.Find("Units"));

                    if (other.GetComponent<Unit>().solarSystemTarget.name == game.currentSolarSystem.Name)
                    {
                        foreach (MeshRenderer mesh in other.transform.GetComponentsInChildren<MeshRenderer>())
                        {
                            mesh.enabled = true;
                        }
                    }
                    else
                    {
                        foreach (MeshRenderer mesh in other.transform.GetComponentsInChildren<MeshRenderer>())
                        {
                            mesh.enabled = false;
                        }
                    }

                    other.GetComponent<Unit>().solarSystemTarget = null;

                    if (Mouse.CurrentlySelectedUnits.Contains(other))
                    {
                        game.GetComponent<Mouse>().RemoveUnitFromCurrentlySelectedUnits(other.gameObject);
                    }

                }
            }
        }
    }
}
