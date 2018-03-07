using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour {

    RaycastHit hit;

	public static GameObject CurrentlySelectedUnit;

	private static Vector3 mouseDownPoint;

    public GameObject Target;

	void Awake()
	{
		mouseDownPoint = Vector3.zero;
	}

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		//Store point at mouse button down
		if (Input.GetMouseButtonDown (0)) 
			mouseDownPoint = hit.point;

		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			Debug.Log (hit.collider.name);
			if (hit.collider.name == "TerrainMain") 
			{
				if (Input.GetMouseButtonDown (1)) {
					GameObject targetObject = Instantiate (Target, hit.point, Quaternion.identity) as GameObject;
					targetObject.name = "Target Instantiated";
				} 
				else if (Input.GetMouseButtonUp (0) && DidUserClickLeftMouse (mouseDownPoint))
					DeselectUnitIfSelected ();
			} // End of Terrain

			else 
			{
				if (Input.GetMouseButtonUp (0) && DidUserClickLeftMouse (mouseDownPoint)) 
				{
					if (hit.collider.transform.Find("Selected").gameObject) {
						Debug.Log ("Found a Unit!");

						if (CurrentlySelectedUnit != hit.collider.gameObject) {
							GameObject SelectedObj = hit.collider.transform.Find("Selected").gameObject;
							SelectedObj.active = true;

							if (CurrentlySelectedUnit != null)
								CurrentlySelectedUnit.transform.Find("Selected").gameObject.active = false;

							CurrentlySelectedUnit = hit.collider.gameObject;
						}
					} 
					else 
					{
						DeselectUnitIfSelected ();
					}
				}
			}
		} 
		else 
		{
			if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
				DeselectUnitIfSelected ();
		}

		Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.yellow);
    }

	#region Helper 

	public bool DidUserClickLeftMouse(Vector3 hitPoint)
	{
		float clickZone = 1.3f;

		if (
			(mouseDownPoint.x < hitPoint.x + clickZone && mouseDownPoint.x > hitPoint.x - clickZone) &&
			(mouseDownPoint.y < hitPoint.y + clickZone && mouseDownPoint.y > hitPoint.y - clickZone) &&
			(mouseDownPoint.z < hitPoint.z + clickZone && mouseDownPoint.z > hitPoint.z - clickZone))
			return true;
		else
			return false;
	}

	public static void DeselectUnitIfSelected()
	{
		if (CurrentlySelectedUnit != null) {
			CurrentlySelectedUnit.transform.Find ("Selected").gameObject.active = false;
			CurrentlySelectedUnit = null;
		}
	}

	#endregion
}
