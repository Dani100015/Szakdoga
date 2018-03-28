using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour {

    public static bool FloatToBool(float f)
    {
        if (f < 0f) return false; else return true;
    }

    public static bool ShiftKeysDown()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            return true;
        else return false;
    }
}
