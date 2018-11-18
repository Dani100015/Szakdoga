using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class GUI_CelestialToolTip : MonoBehaviour
{

    GameObject TipPanel;

    Text TipHeader;
    Text TipInfo;

    string celestialName;
    string celestialInfo;


    Vector3 offset;
    Vector3 activePosition;
    Vector3 deactivePosition;

    bool isPanelActive;
    void Start()
    {
        TipPanel = GameObject.Find("CelestialTipPanel");
        TipHeader = GameObject.Find("CelestialTipPanelHead").transform.Find("CelestialName").GetComponent<Text>();
        TipInfo = GameObject.Find("CelestialInfo").GetComponent<Text>();

        celestialName = transform.name;
        celestialInfo = transform.position.ToString();

        activePosition = new Vector3(120, -120, 0);
        deactivePosition = new Vector3(-800, -800, 0);

        isPanelActive = false;
    }

    void Update()
    {
        
       // Debug.Log(offset);
    }
    void OnMouseEnter()
    {
        TipPanel.transform.localPosition = activePosition;
        TipHeader.text = celestialName;
        TipInfo.text = celestialInfo;
    }

    void OnMouseExit()
    {
        TipPanel.transform.localPosition = deactivePosition;
    }



}
