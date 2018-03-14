using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{

    #region Variables

    RaycastHit hit;

    public static ArrayList CurrentlySelectedUnits = new ArrayList();

    public GUIStyle MouseDragSkin;

    private static Vector3 mouseDownPoint;
    private static Vector3 mouseUpPoint;
    private Vector3 currentMousePoint; //in World Space

    public GameObject Target;

    public static bool UserIsDragging;
    private static float TimeLimitBeforeDeclareDrag = 1f;
    private static float TimeLeftBeforeDeclareDrag;
    private static Vector2 MouseDragStart;
    private static float clickDragZone = 1.3f;

    #endregion

    void Awake()
    {
        mouseDownPoint = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            currentMousePoint = hit.point;
            //Store point at mouse button down
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPoint = hit.point;
                TimeLeftBeforeDeclareDrag = TimeLimitBeforeDeclareDrag;
                MouseDragStart = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                //If the user is not dragging, lets do the tests
                if (!UserIsDragging)
                {
                    TimeLeftBeforeDeclareDrag = Time.deltaTime;
                    if (TimeLeftBeforeDeclareDrag <= 0f || UserDraggingByPosition(MouseDragStart, Input.mousePosition))
                        UserIsDragging = true;
                }

                //User is dragging
                if (UserIsDragging)
                    Debug.Log("Yes, user is dragging");
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (UserIsDragging)
                    Debug.Log("User is no longer dragging");
                TimeLeftBeforeDeclareDrag = 0f;
                UserIsDragging = false;
            }
            //Mouse click
            if (!UserIsDragging)
            {              
                //Debug.Log(hit.collider.name);
                if (hit.collider.name == "TerrainMain")
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        GameObject targetObject = Instantiate(Target, hit.point, Quaternion.identity) as GameObject;
                        targetObject.name = "Target Instantiated";
                    }
                    else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                    {
                        if (!ShiftKeysDown())
                            DeselectGameObjectsIfSelected();
                    }
                } // End of Terrain

                else
                {
                    //Hitting other objects
                    if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                    {
                        //Is the user hitting a unit?
                        if (hit.collider.transform.Find("Selected").gameObject)
                        {
                            //Found a unit we can select
                            Debug.Log("Found a Unit!");

                            //Are we selecting a different object?
                            if (!UnitAlreadyInCurrentySelectedUnits(hit.collider.gameObject))
                            {
                                //If shift key is not down, remove the rest of the units
                                if (!ShiftKeysDown())
                                    DeselectGameObjectsIfSelected();

                                GameObject SelectedObj = hit.collider.transform.Find("Selected").gameObject;
                                SelectedObj.SetActive(true);

                                //Add unit to currently selected units
                                CurrentlySelectedUnits.Add(hit.collider.gameObject);

                            }
                            else
                            {
                                //Unit is currently in the selected units arraylist
                                //Remove the units
                                if (ShiftKeysDown())
                                    RemoveUnitFromCurrentlySelectedUnits(hit.collider.gameObject);
                                else if (!ShiftKeysDown())
                                {
                                    DeselectGameObjectsIfSelected();

                                    GameObject SelectedObj = hit.collider.transform.Find("Selected").gameObject;
                                    SelectedObj.SetActive(true);

                                    //Add unit to currently selected units
                                    CurrentlySelectedUnits.Add(hit.collider.gameObject);
                                }
                            }
                        }
                        else
                        {
                            //If this object is not a unit
                            if (!ShiftKeysDown())
                                DeselectGameObjectsIfSelected();
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                {
                    if (!ShiftKeysDown())
                        DeselectGameObjectsIfSelected();
                }
            } //End of raycasthit
        } //End of dragging
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);
    }

    void OnGUI()
    {
        if (UserIsDragging)
        {
            float BoxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
            float BoxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;

            float BoxLeft = Input.mousePosition.x;
            float BoxTop = (Screen.height - Input.mousePosition.y) - BoxHeight;

            GUI.Box(new Rect(BoxLeft, BoxTop, BoxWidth, BoxHeight), "", MouseDragSkin);
        }
    }

    #region Helper 

    //Is the user dragging relative to mouse drag start point
    public bool UserDraggingByPosition(Vector2 DragStartPoint, Vector2 NewPoint)
    {
        if ((NewPoint.x > DragStartPoint.x + clickDragZone || NewPoint.x < DragStartPoint.x - clickDragZone) ||
            (NewPoint.y > DragStartPoint.y + clickDragZone || NewPoint.y < DragStartPoint.y - clickDragZone)) return true;
        else return false;
    }

    public bool DidUserClickLeftMouse(Vector3 hitPoint)
    {
        if (
            (mouseDownPoint.x < hitPoint.x + clickDragZone && mouseDownPoint.x > hitPoint.x - clickDragZone) &&
            (mouseDownPoint.y < hitPoint.y + clickDragZone && mouseDownPoint.y > hitPoint.y - clickDragZone) &&
            (mouseDownPoint.z < hitPoint.z + clickDragZone && mouseDownPoint.z > hitPoint.z - clickDragZone))
            return true;
        else
            return false;
    }

    public static void DeselectGameObjectsIfSelected()
    {
        if (CurrentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++)
            {
                GameObject ArrayListUnit = CurrentlySelectedUnits[i] as GameObject;
                ArrayListUnit.transform.Find("Selected").gameObject.SetActive(false);
            }
            CurrentlySelectedUnits.Clear();
        }
    }

    public static bool UnitAlreadyInCurrentySelectedUnits(GameObject Unit)
    {
        if (CurrentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++)
            {
                GameObject ArrayListUnit = CurrentlySelectedUnits[i] as GameObject;
                if (ArrayListUnit == Unit)
                    return true;
            }
            return false;
        }
        else return false;
    }

    public void RemoveUnitFromCurrentlySelectedUnits(GameObject Unit)
    {
        if (CurrentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++)
            {
                GameObject ArrayListUnit = CurrentlySelectedUnits[i] as GameObject;
                if (ArrayListUnit == Unit)
                {
                    CurrentlySelectedUnits.RemoveAt(i);
                    ArrayListUnit.transform.Find("Selected").gameObject.SetActive(false);
                }
            }
            return;
        }
        else return;
    }

    public static bool ShiftKeysDown()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            return true;
        else return false;
    }
}

#endregion
