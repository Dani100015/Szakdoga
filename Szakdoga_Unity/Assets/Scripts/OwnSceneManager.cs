using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OwnSceneManager : MonoBehaviour {

    public static void LoadByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
	
}
