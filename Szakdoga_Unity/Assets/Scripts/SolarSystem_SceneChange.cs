using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using RTS_Cam;
using UnityEngine.EventSystems;

public class SolarSystem_SceneChange : MonoBehaviour {

    Game game;
    GameObject gameCanvas;
    Vector3 startPos;
    float viewChangeHeight;

    void Awake()
    {
        gameCanvas = GameObject.Find("GameCanvas");
        game = GameObject.Find("Game").GetComponent<Game>();
    }
    void Start()
    {
     
        startPos = transform.position;
        viewChangeHeight = GetComponent<RTS_Camera>().maxHeight;
        
    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Game.GalaxyView = true;
            CameraViewChange.ChangeCameraView();
        }
	}
}
