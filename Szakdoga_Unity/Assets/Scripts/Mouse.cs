using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{

    #region Variables
    public ArrayList[] Grouping = new ArrayList[10];


    private KeyCode[] keyCodes = {
         KeyCode.Keypad1,
         KeyCode.Keypad2,
         KeyCode.Keypad3,
         KeyCode.Keypad4,
         KeyCode.Keypad5,
         KeyCode.Keypad6,
         KeyCode.Keypad7,
         KeyCode.Keypad8,
         KeyCode.Keypad9,
     };

    RaycastHit hit;

    public Vector3 RightClickPoint;
    public static ArrayList CurrentlySelectedUnits = new ArrayList();
    public static ArrayList UnitsOnScreen = new ArrayList();
    public static ArrayList UnitsInDrag = new ArrayList();
    private bool FinishedDragOnThisFrame;
    private bool StartedDrag;

    public GUIStyle MouseDragSkin;

    private static Vector3 mouseDownPoint;
    private Vector3 currentMousePoint; //in World Space

    public static bool UserIsDragging;
    private static float TimeLimitBeforeDeclareDrag = 1f;
    private static float TimeLeftBeforeDeclareDrag;
    private static Vector2 MouseDragStart;
    private static float clickDragZone = 1.3f;
    public LayerMask MouseLayerMask;

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
                if (Input.GetMouseButtonDown(1))
                {
                    if (hit.collider.gameObject.layer == 8)
                        SelectTargets(hit);
                    else if (hit.collider.name == "TerrainMain")
                    {
                        RightClickPoint = hit.point;
                        DeselectTargets();
                    }

                    else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                    {
                        if (!Common.ShiftKeysDown())
                            DeselectGameObjectsIfSelected();
                    }
                }


                // End of Terrain

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

                                //Change the unit selected value to true
                                hit.collider.gameObject.GetComponent<Unit>().Selected = true;

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
                                    hit.collider.gameObject.GetComponent<Unit>().Selected = true;

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
        //Group creation
        if (CurrentlySelectedUnits.Count != 0 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            for (int i = 1; i < 10; i++)
            {
                if (Input.GetKey(keyCodes[i - 1]))
                {
                    Grouping[i] = new ArrayList();
                    for (int j = 0; j < CurrentlySelectedUnits.Count; j++)
                    {
                        Grouping[i].Add(CurrentlySelectedUnits[j]);
                    }
                    Debug.Log(Grouping[i].Count);
                }
            }
        }

        //Group selection
        if (GetAnyKey(keyCodes))
        {
            for (int i = 1; i < 10; i++)
            {
                if (Input.GetKey(keyCodes[i - 1]) && Grouping[i] != null)
                {
                    DeselectGameObjectsIfSelected();
                    for (int j = 0; j < Grouping[i].Count; j++)
                    {
                        CurrentlySelectedUnits.Add((Grouping[i])[j]);
                    }
                }
            }
            Debug.Log(CurrentlySelectedUnits.Count);
            for (int j = 0; j < CurrentlySelectedUnits.Count; j++)
            {
                GameObject SelectedObj = CurrentlySelectedUnits[j] as GameObject;
                SelectedObj.GetComponent<Unit>().Selected = true;
                SelectedObj.transform.Find("Selected").gameObject.SetActive(true);
                Debug.Log(j + ". aktív");
            }
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
                if (UnitObj != null)
                {
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
    bool GetAnyKey(KeyCode[] aKeys)
    {
        foreach (var key in aKeys)
            if (Input.GetKey(key))
                return true;
        return false;
    }

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
                if (ArrayListUnit != null)
                {
                    ArrayListUnit.transform.Find("Selected").gameObject.SetActive(false);
                    ArrayListUnit.GetComponent<Unit>().Selected = false;
                }
            }
        }
        CurrentlySelectedUnits.Clear();
    }

    public static void SelectTargets(RaycastHit hit)
    {
        if (CurrentlySelectedUnits.Count != 0)
        {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++)
            {
                GameObject CurrentObject = CurrentlySelectedUnits[i] as GameObject;
                if (CurrentObject != null)
                    CurrentObject.GetComponent<AIDestinationSetter>().target = hit.collider.gameObject.transform;
            }
        }
    }

    public static void DeselectTargets()
    {
        if (CurrentlySelectedUnits.Count != 0)
        {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++)
            {
                GameObject CurrentObject = CurrentlySelectedUnits[i] as GameObject;
                if (CurrentObject != null && CurrentObject.GetComponent<AIDestinationSetter>() != null)
                    CurrentObject.GetComponent<AIDestinationSetter>().target = null;
            }
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
           (UnitScreenPos.x < BoxFinish.x && UnitScreenPos.y > BoxFinish.y))
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
