using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    //for Mouse.cs
    public Vector2 ScreenPos;
    public bool OnScreen;
    public bool Selected = false;

    public bool isWalkable = true;

    //void Awake()
    //{
    //    Physics.IgnoreLayerCollision(8,8,true);
    //}

    void Update()
    {
        //if unit not selected, get screenspace
        if (!Selected)
        {
            //track the screen position
            ScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);

            //if within screen space
            if (Mouse.UnitWithinScreenSpace(ScreenPos))
            {
                //and not already added to UnitsOnScreen, add it
                if (!OnScreen)
                {
                    Mouse.UnitsOnScreen.Add(this.gameObject);
                    OnScreen = true;
                }
            } //unit is not in screen space
            else
            {
                //remove if previously on screen
                if (OnScreen)
                {
                    Mouse.RemoveFromOnScreenUnits(this.gameObject);
                }
            }
        }
    }
}
