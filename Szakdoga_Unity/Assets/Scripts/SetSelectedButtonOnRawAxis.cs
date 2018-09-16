using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetSelectedButtonOnRawAxis : MonoBehaviour {

    public GameObject selectedButtonGameObejct;
    public EventSystem eventSystem;

    private bool buttonSelected;

	void Update () {

        if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            eventSystem.SetSelectedGameObject(selectedButtonGameObejct);
            buttonSelected = true;
        }
    }

    void OnDisable()
    {
        buttonSelected = false;
    }
}
