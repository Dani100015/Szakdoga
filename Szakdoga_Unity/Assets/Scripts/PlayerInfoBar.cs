using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoBar : MonoBehaviour {

    Player player;

    public Text resourse1_Palladium;
    public Text resourse2_Iridium;
    public Text resourse3_NullElement;



    void Start()
    {
        player = new Player();
    }
    void Update()
    {
        resourse1_Palladium.text    = "Palladium: " +   player.Palladium.ToString();
        resourse2_Iridium.text      = "Iridium: " +     player.Iridium.ToString();
        resourse3_NullElement.text  = "Null Element: "+ player.NullElement.ToString();
    }

    void OnMouseEnter()
    {
        //Honann vannak az erőforrások?
    }

    void OnMouseExit()
    {

    }
}
