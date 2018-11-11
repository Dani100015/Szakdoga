using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoBar : MonoBehaviour
{

    public Text resource1_Palladium;
    public Text resource2_Iridium;
    public Text resource3_NullElement;

    void Awake()
    {
        resource1_Palladium = GameObject.Find("TextPalladium").GetComponent<Text>();
        resource2_Iridium = GameObject.Find("TextIridium").GetComponent<Text>();
        resource3_NullElement = GameObject.Find("TextNullElement").GetComponent<Text>();
    }
    void LateUpdate()
    {
        //resource1_Palladium.text = "Palladium: " + Game.currentPlayer.palladium;
        //resource2_Iridium.text = "Iridium: " + Game.currentPlayer.iridium;
        //resource3_NullElement.text = "Null Element: " + Game.currentPlayer.nullElement;
    }

    void OnMouseEnter()
    {
        //Honann vannak az erőforrások?
    }

    void OnMouseExit()
    {
        //
    }
}
