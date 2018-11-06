using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class TechTreeControl : MonoBehaviour {

    GameObject[] techCategories;
    GameObject techPanel;

    //Image[] techPanelImages

    void Start()
    {
        techPanel = GameObject.Find("TechPanel");
        techCategories = GameObject.FindGameObjectsWithTag("TechCategory");

        for (int i = 0; i < techCategories.Length; i++)  //Other category deactivate
        {
            techCategories[i].SetActive(false);
        }
        techCategories[0].SetActive(true);  //Firt category active

        techPanel.SetActive(false);
    }

    void Update()
    {

    }

    public void SetActiveCurrentTechCategory(GameObject currentTechCategory)
    {
        for (int i = 0; i < techCategories.Length; i++)
        {
            techCategories[i].SetActive(false);
        }
        currentTechCategory.SetActive(true);

    }

    public void SetActiveTechPanel(GameObject techPanel)
    {

        if (techPanel.activeSelf == false)
        {
            techPanel.SetActive(true);
        }
        else if (techPanel.activeSelf == true)
        {
            techPanel.SetActive(false);
        }
    }
}
