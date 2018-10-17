using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipLocation : MonoBehaviour {

    public float yPosition;
    public float xPosition;

	void Update () {
        this.gameObject.transform.position = new Vector2(Input.mousePosition.x + xPosition, Input.mousePosition.y + yPosition);
	}
}
