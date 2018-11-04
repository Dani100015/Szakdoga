using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_BillBoard : MonoBehaviour {

    public Camera myCamera;

    void Start()
    {
        myCamera = Camera.main;
    }
	void Update () {
        transform.LookAt(transform.position + myCamera.transform.rotation * new Vector3(1f, 0.2f, 2.2f),
                        myCamera.transform.rotation * Vector3.one);

	}
}
