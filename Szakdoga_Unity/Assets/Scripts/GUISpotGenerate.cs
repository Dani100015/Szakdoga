using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISpotGenerate : MonoBehaviour {

    public GameObject ContainerRect;//A panel ahova kerülnek a "spot"-ok
    public GameObject spotPrefab;   //A "spot" Gameobject
    public int spotLimitNumber;     //A panelekbe csak limitált mennyiségű "spotot"/üres helyet lehet berakni
    public int spotNumber;          //Input "spot" generálási mennyiség, 

    /// <summary>
    /// Spotokat/"Üres helyket generál a külöböző GUI-hoz, attól függően, hogy melyik Panel-ben található
    /// </summary>
    public void GenerateSpots()
    {
        switch (ContainerRect.gameObject.name)  //Attól függően melyik Panelbe kell generálni. (Név alapján)
        {
            case "UnitDetail":
                {

                    transform.Find("UnitName").GetComponent<Text>().text = Mouse.CurrentlyFocusedUnit.name;
                    //transform.Find("UnitName").GetComponent<Text>().text = Mouse.CurrentlyFocusedUnit.name;

                }
                break;
            case "UnitList":
                {                    
                        for (int i = 0; i < 30; i++)
                        {
                            GameObject scrollItemObject = Instantiate(spotPrefab,ContainerRect.transform);
                        }
                }
                break;
            case "CommandSpots":
                {
                    if (spotNumber <= spotLimitNumber)
                    {
                        for (int i = 0; i < spotNumber; i++)
                        {
                            GameObject scrollItemObject = Instantiate(spotPrefab);
                            scrollItemObject.transform.SetParent(ContainerRect.transform, false);
                        }
                    }

                } break;
            default:
                break;
        }      
    }
}
