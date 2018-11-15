using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CelestialToolTip   : MonoBehaviour {

    GameObject TipPanel;

    Text TipHeader;
    Text TipInfo;

    string celestialName;
    string celestialInfo;

    Vector3 activePosition;
    Vector3 deactivePosition;
	void Start () {
        TipPanel = GameObject.Find("CelestialTipPanel");
        TipHeader = GameObject.Find("CelestialTipPanelHead").transform.Find("CelestialName").GetComponent<Text>();
        TipInfo = GameObject.Find("CelestialInfo").GetComponent<Text>();

        celestialName = transform.name;
        celestialInfo = transform.position.ToString();

        activePosition = new Vector3(100, 100, 0);
        deactivePosition = TipPanel.transform.localPosition;
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
