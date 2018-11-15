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
    void Start()
    {
        TipPanel = GameObject.Find("CelestialTipPanel");
        TipHeader = GameObject.Find("CelestialTipPanelHead").transform.Find("CelestialName").GetComponent<Text>();
        TipInfo = GameObject.Find("CelestialInfo").GetComponent<Text>();

        celestialName = transform.name;
        celestialInfo = transform.position.ToString();

        activePosition = new Vector3(120, -120, 0);
        deactivePosition = TipPanel.transform.position;

        bool isPanelActive = false;
    }

    void Update()
    {
        
        Debug.Log(offset);
    }
    void OnMouseDown()
    {
        if (TipPanel.transform.position == deactivePosition)
        {
            offset = Input.mousePosition;

            TipPanel.transform.position = offset + activePosition;
            TipHeader.text = celestialName;
            TipInfo.text = celestialInfo;
        }
        else if (TipPanel.transform.position == activePosition)
        {
            TipPanel.transform.position = deactivePosition;
        }
    }



}
