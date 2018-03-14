using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePoint : MonoBehaviour {

    RaycastHit hit;
    public GameObject Target;
    private float rayCastLength = 500;

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, rayCastLength))
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.name == "TerrainMain")
            {
                if (Input.GetMouseButtonDown(1))
                {
                    GameObject targetObject = Instantiate(Target, hit.point, Quaternion.identity) as GameObject;
                    targetObject.name = "Target Instantiated";
                }
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * rayCastLength, Color.yellow);
    }
}
