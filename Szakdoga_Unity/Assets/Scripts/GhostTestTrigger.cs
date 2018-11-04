using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTestTrigger : MonoBehaviour {

    void Start()
    {
        GUISetup.PassedTriggerTest = true;
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Structure>())
        {
            GUISetup.PassedTriggerTest = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Structure>())
        {
            GUISetup.PassedTriggerTest = true;
        }
    }
}
