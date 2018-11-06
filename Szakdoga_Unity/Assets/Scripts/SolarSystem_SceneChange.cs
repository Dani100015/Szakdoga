using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using RTS_Cam;

public class SolarSystem_SceneChange : MonoBehaviour {

    Game game;
    Vector3 startPos;
    float viewChangeHeight;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        startPos = transform.position;
        viewChangeHeight = GetComponent<RTS_Camera>().maxHeight;
        
    }

    void Update () {

        if (transform.position.y > viewChangeHeight)
        {
            GameObject.DontDestroyOnLoad(game);
            SceneManager.LoadScene("Galaxy");
        }
	}
}
