using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class OwnSceneManager : MonoBehaviour {

    public void LoadByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
	
}
