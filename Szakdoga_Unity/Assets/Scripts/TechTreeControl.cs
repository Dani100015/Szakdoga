using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class TechTreeControl : MonoBehaviour {

    GameObject[] techCategories;
    GameObject techPanel;

    //Image[] techPanelImages

    Vector3 activePosition;
    Vector3 deactivePosition;
    void Start()
    {
        techPanel = GameObject.Find("TechPanel");
        techCategories = GameObject.FindGameObjectsWithTag("TechCategory");

        for (int i = 0; i < techCategories.Length; i++)  //Other category deactivate
        {
            techCategories[i].SetActive(false);
        }
        techCategories[0].SetActive(true);  //Firt category active

        activePosition = new Vector3(80, 100 , 0f);
        deactivePosition = techPanel.transform.localPosition;

        techPanel.transform.localPosition = deactivePosition;
        //Debug.Log("Tech position: " + techPanel.transform.localPosition);

    }

    public void SetActiveCurrentTechCategory(GameObject currentTechCategory)
    {
        for (int i = 0; i < techCategories.Length; i++)
        {
            techCategories[i].transform.gameObject.SetActive(false);
        }
        currentTechCategory.SetActive(true);
    }

    public void SetActiveTechPanel(GameObject techPanel)
    {

        //Debug.Log(techPanel.transform.position);
        if (techPanel.transform.localPosition == deactivePosition)
        {
            techPanel.transform.localPosition = activePosition;
        }
        else if (techPanel.transform.localPosition == activePosition)
        {
            techPanel.transform.localPosition = deactivePosition;
        }
    }


}
