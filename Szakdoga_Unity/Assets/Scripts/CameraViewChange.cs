using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewChange : MonoBehaviour {

    static Camera solarSystemViewCamera;
    static Camera galaxyViewCamera;

    static Vector3 galaxyViewCameraDefaultPosition = new Vector3(1200,300,900);
    static Vector3 solarSystemViewCameraDefaultPosition = new Vector3(-75,150,-125);

    static AudioListener solarSystemAudioListener;
    static AudioListener galaxyAudioListener;

    // Use this for initialization
	void Start () {

        solarSystemViewCamera = GameObject.Find("SolarSystemCamera").GetComponent<Camera>();
        galaxyViewCamera = GameObject.Find("GalaxyCamera").GetComponent<Camera>();

        solarSystemAudioListener = GameObject.Find("SolarSystemCamera").GetComponent<AudioListener>();
        galaxyAudioListener = GameObject.Find("GalaxyCamera").GetComponent<AudioListener>();

        solarSystemViewCamera.gameObject.SetActive(true);
        galaxyViewCamera.gameObject.SetActive(false);
	}
	
    public static void ChangeCameraView()
    {
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
