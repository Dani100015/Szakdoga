using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using RTS_Cam;

public class SolarSystem_SceneChange : MonoBehaviour {

    Vector3 startPos;
    float viewChangeHeight;

    void Start()
    {
        startPos = transform.position;
        viewChangeHeight = GetComponent<RTS_Camera>().maxHeight * 2 - 5;
    }

    void Update () {
        if (transform.position.y > viewChangeHeight)
        {
            SceneManager.LoadScene("Galaxy");
        }
	}
}
