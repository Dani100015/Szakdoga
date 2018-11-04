using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMeshTrigger : MonoBehaviour
{
    //TriggeredUnits stores triggered units on every event
    public static ArrayList TriggeredUnits = new ArrayList(); //Gameobjects

    //SelectedUnits stores units from the previous frame
    public static ArrayList SelectedUnits = new ArrayList();

    //NewSelectedUnits stores the new selection before overwriting the SelectedUnits arraylist
    public static ArrayList NewSelectedUnits = new ArrayList();

    public static bool NewTriggerEvent = false;

    public void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 11:
                NewTriggerEvent = true;
                //If unit is not already selected, add it to TriggeredUnits
                GameObject UnitGameObject = other.transform.parent.gameObject;
                if (!UnitAlreadyInTriggeredUnits(UnitGameObject) && !Mouse.UnitAlreadyInCurrentySelectedUnits(UnitGameObject))
                    TriggeredUnits.Add(UnitGameObject);
                break;
            case 12:
                NewTriggerEvent = true;
                break;
            default: break;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 11:
                NewTriggerEvent = true;
                //If unit is not already selected, add it to TriggeredUnits
                GameObject UnitGameObject = other.transform.parent.gameObject;
                if (!UnitAlreadyInTriggeredUnits(UnitGameObject) && !Mouse.UnitAlreadyInCurrentySelectedUnits(UnitGameObject))
                    TriggeredUnits.Add(UnitGameObject);
                break;
            case 12:
                NewTriggerEvent = true;
                break;
            default: break;
        }
    }

    #region Helper
    //Is unit in the SelectedUnits arraylist
    public static bool UnitAlreadyInDragMesh(GameObject Unit)
    {
        if (SelectedUnits.Count == 0)
            return false;
        for (int i = 0; i < SelectedUnits.Count; i++)
        {
            GameObject UnitObj = SelectedUnits[i] as GameObject;
            if (UnitObj == Unit)
                return true;
        }
        return false;
    }

    //Is unit in the TriggeredUnits arraylist
    public static bool UnitAlreadyInTriggeredUnits(GameObject Unit)
    {
        if (SelectedUnits.Count == 0)
            return false;
        for (int i = 0; i < TriggeredUnits.Count; i++)
        {
            GameObject UnitObj = TriggeredUnits[i] as GameObject;
            if (UnitObj == Unit)
                return true;
        }
        return false;
    }
    #endregion
}
