using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour {

    [SerializeField] private float speed;

	void Update ()
    {

		if (Input.GetKey("d"))
        {
            transform.position += new Vector3(speed * Time.deltaTime , 0f,0f);
        }
        if (Input.GetKey("a"))
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey("w"))
        {
            transform.position += new Vector3(0f, 0f, speed * Time.deltaTime); ;         
        }
        if (Input.GetKey("s"))
        {
            transform.position -= new Vector3(0f, 0f, speed * Time.deltaTime);
        }
    }
}
