using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_ToolTip : MonoBehaviour {

    [Header("Object Text Information")]
    public string objectName;
    [TextArea]
    public string objectInfo;

    [Header("Display the information")]
    public UnityEngine.GameObject toolTipWindow;
    public Text displayName;
    public Text displayInformation;

    void OnMouseEnter()
    {
        toolTipWindow.SetActive(true);

        if (toolTipWindow !=  null)
        {
            displayName.text =        objectName;
            displayInformation.text = objectInfo;
        }
    }

    void OnMouseExit()
    {
        toolTipWindow.SetActive(false);
    }
}
