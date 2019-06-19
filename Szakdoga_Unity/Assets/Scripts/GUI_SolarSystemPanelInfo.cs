using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class GUI_SolarSystemPanelInfo : MonoBehaviour
{

    GameObject[] solarSystems;
    Text SolarSystemNameText;

    SetSolarSystems setSystems;
    Game game;

    Vector3 activePosition;
    Vector3 deactivePosition;

    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        SolarSystemNameText = transform.Find("TextSolarSystem").GetComponent<Text>();
    }

    void Start()
    {
        activePosition = new Vector3(0, 400, 0f);
        deactivePosition = new Vector3(1000,1000,0);

        transform.localPosition = activePosition;
    }
    void Update()
    {
            //InvokeRepeating("UpdatetSolarSystemText", 1, 2);
            if (Game.GalaxyView == true)
            {
                transform.localPosition = deactivePosition;
            }
            if (Game.GalaxyView == false)
            {
                transform.localPosition = activePosition;
                if (game != null && game.currentSolarSystem != null)
                {
                    SolarSystemNameText.text = game.currentSolarSystem.Name;
                }

            }
    }
    //void UpdatetSolarSystemText()
    //{
    //    if (Game.GalaxyView == true)
    //    {
    //        transform.localPosition = deactivePosition;
    //    }
    //    if (Game.GalaxyView == false)
    //    {
    //        transform.localPosition = activePosition;
    //        if (game != null && game.currentSolarSystem != null)
    //        {
    //            SolarSystemNameText.text = game.currentSolarSystem.Name;
    //        }

    //    }
    //}
}
