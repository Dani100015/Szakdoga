using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewChange : MonoBehaviour {

    //Camerák
    static Camera solarSystemViewCamera;
    static Camera galaxyViewCamera;

    //Camera alap alapértelmezett pozíciók
    static Vector3 galaxyViewCameraDefaultPosition = new Vector3(1200,300,900);
    static Vector3 solarSystemViewCameraDefaultPosition = new Vector3(-75,150,-125);

    //Camera Audio Componensek
    static AudioListener solarSystemAudioListener;
    static AudioListener galaxyAudioListener;

	void Start () {

        //Változó inicializálása
        solarSystemViewCamera = GameObject.Find("SolarSystemCamera").GetComponent<Camera>();
        galaxyViewCamera = GameObject.Find("GalaxyCamera").GetComponent<Camera>();
        solarSystemAudioListener = GameObject.Find("SolarSystemCamera").GetComponent<AudioListener>();
        galaxyAudioListener = GameObject.Find("GalaxyCamera").GetComponent<AudioListener>();

        //Beálítjuk a kezdőkamerát
        solarSystemViewCamera.gameObject.SetActive(true);
        galaxyViewCamera.gameObject.SetActive(false);
	}
	
    /// <summary>
    /// Kamera nézet váltás
    /// </summary>
    public static void ChangeCameraView()
    {
        //Meg kell vizsgálni melyik kamera aktív

        if (solarSystemViewCamera.gameObject.activeSelf == true)
        {
            solarSystemViewCamera.gameObject.SetActive(false);
            solarSystemAudioListener.enabled = false;

            galaxyViewCamera.gameObject.SetActive(true);
            galaxyAudioListener.enabled = true;

            galaxyViewCamera.transform.localPosition = galaxyViewCameraDefaultPosition;
            Game.mainCamera = galaxyViewCamera;
          

        }
        else if (galaxyViewCamera.gameObject.activeSelf == true)
        {
            galaxyViewCamera.gameObject.SetActive(false);
            galaxyAudioListener.enabled = false;

            solarSystemViewCamera.gameObject.SetActive(true);
            solarSystemAudioListener.enabled = true;
            
            solarSystemViewCamera.transform.localPosition = solarSystemViewCameraDefaultPosition;
            Game.mainCamera = solarSystemViewCamera;

        }
    }
}
