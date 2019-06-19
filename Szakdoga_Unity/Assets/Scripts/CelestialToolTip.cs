using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CelestialToolTip : MonoBehaviour {

    //Panel gameobejct
    GameObject TipPanel;

    //UI Text elemek
    Text TipHeader;
    Text TipInfo;

    //Megjelenítendő információk a panelen
    string celestialName;
    string celestialInfo;

    //Panel pozíciók
    Vector3 activePosition;
    Vector3 deactivePosition;
	void Start () {
        TipPanel  = GameObject.Find("CelestialTipPanel");
        TipHeader = GameObject.Find("CelestialTipPanelHead").transform.Find("CelestialName").GetComponent<Text>();
        TipInfo   = GameObject.Find("CelestialInfo").GetComponent<Text>();

        celestialName = transform.name;
        celestialInfo = transform.position.ToString();

        activePosition = new Vector3(100, 100, 0);
        deactivePosition = TipPanel.transform.localPosition;
    }

    void OnMouseOver()
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
