using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class SolarSystemPanelInfo : MonoBehaviour {

    GameObject[] solarSystems;
    public Text SolarSystemName;

    void Start()
    {
        solarSystems = GameObject.FindGameObjectsWithTag("StarSystem");
    }
    void Update () {
        for (int i = 0; i < solarSystems.Length; i++)
        {
            if (solarSystems[i].gameObject.activeSelf == true)
            {
                SolarSystemName.text = solarSystems[0].gameObject.name;
            }
            Debug.Log(solarSystems[0].gameObject.name);
        }
        
    }
}
