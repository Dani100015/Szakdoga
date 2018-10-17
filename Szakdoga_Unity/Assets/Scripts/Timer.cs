using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Timer : MonoBehaviour {

    public Text gameTime;
    float minutes, seconds, hours;
    
	void Start () {

	}
	
	void Update () {
        hours = (int)(Time.time / 120);
        minutes = (int)(Time.time / 60f);
        seconds = (int)(Time.time % 60f);

        //gameTime.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        gameTime.text = Time.timeSinceLevelLoad.ToString("00:00:00");
	}
}
