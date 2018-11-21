using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour {

    GameObject menuPanel;

    Vector3 activePosition;
    Vector3 deactivePosition;

    void Awake()
    {

    }

    void Start () {

        //menuPanel = GameObject.Find("MenuPanel");

        //activePosition = new Vector3(0f, 0f, 0f);
        //deactivePosition = new Vector3(1000,1000,0f);

        //menuPanel.transform.localPosition = deactivePosition;
    }
	

    public void SetActiveMenuPanel(GameObject menuPanel)
    {
        if (menuPanel.transform.localPosition.x - deactivePosition.x == 0f)
        {
            menuPanel.transform.localPosition = activePosition;
        }
        else if (menuPanel.transform.localPosition.x - activePosition.x == 0f)
        {
            menuPanel.transform.localPosition = deactivePosition;
        }

    }

}
