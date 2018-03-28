using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{

    #region Variables

    RaycastHit hit;

    public static ArrayList CurrentlySelectedUnits = new ArrayList();
    public static ArrayList UnitsOnScreen = new ArrayList();
    public static ArrayList UnitsInDrag = new ArrayList();
    private bool FinishedDragOnThisFrame;
    private bool StartedDrag;

    public GUIStyle MouseDragSkin;

    private static Vector3 mouseDownPoint;
    private Vector3 currentMousePoint; //in World Space

    public GameObject Target;

    public static bool UserIsDragging;
    private static float TimeLimitBeforeDeclareDrag = 1f;
    private static float TimeLeftBeforeDeclareDrag;
    private static Vector2 MouseDragStart;
    private static float clickDragZone = 1.3f;

    //GUI
    private float BoxWidth;
    private float BoxHeight;
    private float BoxTop;
    private float BoxLeft;
    private static Vector2 BoxStart;
    private static Vector2 BoxFinish;

    #endregion
      
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
                StartedDrag = true;
                
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
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //User is dragging
                if (UserIsDragging)
                    FinishedDragOnThisFrame = true;
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
                        if (!Common.ShiftKeysDown())
                            DeselectGameObjectsIfSelected();
                    }
                } // End of Terrain

                else
                {
                    //Hitting other objects
                    if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                    {
                        //Is the user hitting a unit?
                        if (hit.collider.gameObject.GetComponent<Unit>())
                        {                        
                            //Are we selecting a different object?
                            if (!UnitAlreadyInCurrentySelectedUnits(hit.collider.gameObject))
                            {
                                //If shift key is not down, remove the rest of the units
                                if (!Common.ShiftKeysDown())
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
                                if (Common.ShiftKeysDown())
                                    RemoveUnitFromCurrentlySelectedUnits(hit.collider.gameObject);
                                else if (!Common.ShiftKeysDown())
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
                            if (!Common.ShiftKeysDown())
                                DeselectGameObjectsIfSelected();
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                {
                    if (!Common.ShiftKeysDown())
                        DeselectGameObjectsIfSelected();
                }
            } //End of raycasthit
        } //End of dragging

        if (!Common.ShiftKeysDown() && StartedDrag && UserIsDragging)
        {
            DeselectGameObjectsIfSelected();
            StartedDrag = false;
        }
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);
        //GUI Variables

        if (UserIsDragging)
        {
            BoxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
            BoxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;

            BoxLeft = Input.mousePosition.x;
            BoxTop = (Screen.height - Input.mousePosition.y) - BoxHeight;

            if (Common.FloatToBool(BoxWidth))
                if (Common.FloatToBool(BoxHeight))
                    BoxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + BoxHeight);
                else
                    BoxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            else if (!Common.FloatToBool(BoxWidth))
                if (Common.FloatToBool(BoxHeight))
                    BoxStart = new Vector2(Input.mousePosition.x + BoxWidth, Input.mousePosition.y + BoxHeight);
                else
                    BoxStart = new Vector2(Input.mousePosition.x + BoxWidth, Input.mousePosition.y);

            BoxFinish = new Vector2(BoxStart.x + Mathf.Abs(BoxWidth), BoxStart.y - Mathf.Abs(BoxHeight));
        }           
    }

    void LateUpdate()
    {
        UnitsInDrag.Clear();

        if ((UserIsDragging || FinishedDragOnThisFrame) && UnitsOnScreen.Count > 0)
        {
            //Loop through units on the screen
            for (int i = 0; i < UnitsOnScreen.Count; i++)
            {
                GameObject UnitObj = UnitsOnScreen[i] as GameObject;
                Unit UnitScript = UnitObj.GetComponent<Unit>();
                GameObject SelectedObj = UnitObj.transform.Find("Selected").gameObject;

                //If not already in the dragged units
                if (!UnitAlreadyInDraggedUnits(UnitObj))
                {
                    if (UnitInsideDrag(UnitScript.ScreenPos))
                    {
                        SelectedObj.SetActive(true);
                        UnitsInDrag.Add(UnitObj);
                    } //unit is not in drag
                    else
                    {
                        //remove the selected graphic, if unit is not already in CurrentlySelectedUnits
                        if (!UnitAlreadyInCurrentySelectedUnits(UnitObj))
                            SelectedObj.SetActive(false);
                    }
                }
            }
        }

        if (FinishedDragOnThisFrame)
        {
            FinishedDragOnThisFrame = false;
            PutDraggedUnitsInCurrentlySelectedUnits();
        }
        
    }

    void OnGUI()
    {
        if (UserIsDragging)
        {
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
                ArrayListUnit.GetComponent<Unit>().Selected = false;
            }
           
        }
        CurrentlySelectedUnits.Clear();
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
   
    //Check if unit is within screen space
    public static bool UnitWithinScreenSpace(Vector2 UnitScreenPos)
    {
        if ((UnitScreenPos.x < Screen.width && UnitScreenPos.y < Screen.height) && (UnitScreenPos.x > 0f && UnitScreenPos.y > 0f))
            return true;
        else return false;
    }

    public static void RemoveFromOnScreenUnits(GameObject Unit)
    {
        for (int i = 0; i < UnitsOnScreen.Count; i++)
        {
            GameObject UnitObj = UnitsOnScreen[i] as GameObject;
            if (Unit == UnitObj)
            {
                UnitsOnScreen.RemoveAt(i);
                UnitObj.GetComponent<Unit>().OnScreen = false;
                return;
            }
            return;
        }
    }

    //Is the unit inside drag?
    public static bool UnitInsideDrag(Vector2 UnitScreenPos)
    {
        if ((UnitScreenPos.x > BoxStart.x && UnitScreenPos.y < BoxStart.y) && 
           (UnitScreenPos.x <BoxFinish.x && UnitScreenPos.y > BoxFinish.y))
            return true;
        else return false;
    }

    //Check if unit is in UnitsInDrag array list
    public static bool UnitAlreadyInDraggedUnits(GameObject Unit)
    {
        if (UnitsInDrag.Count > 0)
        {
            for (int i = 0; i < UnitsInDrag.Count; i++)
            {
                GameObject ArrayListUnit = UnitsInDrag[i] as GameObject;
                if (ArrayListUnit == Unit)
                    return true;
            }
            return false;
        }
        else return false;
    }

    //Take all units from UnitsInDrag into CurrentlySelected Units
    public static void PutDraggedUnitsInCurrentlySelectedUnits()
    {
        if (UnitsInDrag.Count > 0)
        {
            for (int i = 0; i < UnitsInDrag.Count; i++)
            {
                GameObject UnitObj = UnitsInDrag[i] as GameObject;

                //If unit is not in CurrentlySelectedUnits, add it
                if (!UnitAlreadyInCurrentySelectedUnits(UnitObj))
                {
                    CurrentlySelectedUnits.Add(UnitObj);
                    UnitObj.GetComponent<Unit>().Selected = true;
                }             
            }
            UnitsInDrag.Clear();
        }
    }
}

#endregion
