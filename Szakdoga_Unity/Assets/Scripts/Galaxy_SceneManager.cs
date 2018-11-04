using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Galaxy_SceneManager : MonoBehaviour {


    GameObject selectedObject;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                if (selectedObject != hitInfo.transform.gameObject)
                {
                    selectedObject = hitInfo.transform.gameObject;
                }
                else
                {
                    SceneManager.LoadScene(selectedObject.name.ToString());
                }
            }
        }
    }
}
