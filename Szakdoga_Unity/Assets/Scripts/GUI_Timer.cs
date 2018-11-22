using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class GUI_Timer : MonoBehaviour {

    public Text gameTime;
    float minutes, seconds, hours;
    
	void Start () {

	}
	
	void Update () {
        hours = (int)(Time.time / 120);
        minutes = (int)(Time.time / 60f);
        seconds = (int)(Time.time % 60f);
        gameTime.text = Time.timeSinceLevelLoad.ToString("00:00:00");
	}
}
