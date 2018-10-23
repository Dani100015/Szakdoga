using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
    public struct ClipPlainPoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

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
    public static GameObject CurrentlyFocusedUnit;
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

    //DragMesh
    public LayerMask SelectMeshLayerMask;
    public float DistanceToGround;
    public LayerMask TerrainOnly;
    public GameObject DragSelectMesh;
    public GameObject Pointer;
    public Material DragSelectMeshMat;

    #endregion

    void Start()
    {
        Pointer = new GameObject();
        Pointer.name = "PointerForDragMesh";
        CreateDragBoxMesh();
    }

    // Update is called once per frame
    void Update()
    {
        ClipPlainPoints NearPlainPoints = CameraClipPlanePoints(Camera.main.GetComponent<Camera>().nearClipPlane);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, MouseLayerMask) && !(EventSystem.current.IsPointerOverGameObject(-1)))
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
                if (DragSelectMesh.activeSelf == false)
                    DragSelectMesh.SetActive(true);
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
                DragSelectMesh.SetActive(false);                               
            }

            //Mouse click
            if (!UserIsDragging)
            {
                //Debug.Log(hit.collider.name);
                if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
                        SelectTargets(hit);
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Resources"))
                    {
                        Debug.Log("Hit Resource");
                        SelectGatherTargets(hit);
                    }
                    else if (hit.collider.name == "TerrainMain")
                    {
                        SelectTargets(hit);
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
                        if (hit.collider.gameObject.GetComponent<Unit>() || hit.collider.gameObject.layer == LayerMask.NameToLayer("SelectMesh"))
                        {
                            Transform UnitGameObject;
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("SelectMesh"))
                                UnitGameObject = hit.collider.transform.parent.transform;
                            else
                                UnitGameObject = hit.collider.transform;

                            //Are we selecting a different object?
                            if (!UnitAlreadyInCurrentySelectedUnits(UnitGameObject.gameObject))
                            {
                                //If shift key is not down, remove the rest of the units
                                if (!Common.ShiftKeysDown())
                                    DeselectGameObjectsIfSelected();

                                GameObject SelectedObj = UnitGameObject.Find("Selected").gameObject;
                                SelectedObj.SetActive(true);

                                //Add unit to currently selected units
                                CurrentlySelectedUnits.Add(UnitGameObject.gameObject);

                                //Change the unit selected value to true
                                UnitGameObject.gameObject.GetComponent<Unit>().Selected = true;

                            }
                            else
                            {
                                //Unit is currently in the selected units arraylist
                                //Remove the units
                                if (Common.ShiftKeysDown())
                                    RemoveUnitFromCurrentlySelectedUnits(UnitGameObject.gameObject);
                                else if (!Common.ShiftKeysDown())
                                {
                                    DeselectGameObjectsIfSelected();

                                    GameObject SelectedObj = UnitGameObject.Find("Selected").gameObject;
                                    SelectedObj.SetActive(true);
                                    UnitGameObject.gameObject.GetComponent<Unit>().Selected = true;

                                    //Add unit to currently selected units
                                    CurrentlySelectedUnits.Add(UnitGameObject.gameObject);
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
            }
            //End of dragging            
        } //End of raycasthit

        if (CurrentlySelectedUnits.Count != 0)
            CurrentlyFocusedUnit = CurrentlySelectedUnits[0] as GameObject;
        else CurrentlyFocusedUnit = null;

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

        if (UserIsDragging && currentMousePoint != Vector3.positiveInfinity)
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
            else
                if (!Common.FloatToBool(BoxWidth))
            {
                if (Common.FloatToBool(BoxHeight))
                    BoxStart = new Vector2(Input.mousePosition.x + BoxWidth, Input.mousePosition.y + BoxHeight);
                else
                    BoxStart = new Vector2(Input.mousePosition.x + BoxWidth, Input.mousePosition.y);
            }

            BoxFinish = new Vector2(BoxStart.x + Mathf.Abs(BoxWidth), BoxStart.y - Mathf.Abs(BoxHeight));
        }
    }

    void LateUpdate()
    {
        UnitsInDrag.Clear();
       
        if ((UserIsDragging || FinishedDragOnThisFrame) && UnitsOnScreen.Count > 0)
        {
            UpdateDragBoxMesh();
            //Loop through units on the screen
            for (int i = 0; i < UnitsOnScreen.Count; i++)
            {
                GameObject UnitObj = UnitsOnScreen[i] as GameObject;
                if (UnitObj != null)
                {
                    Unit UnitScript = UnitObj.GetComponent<Unit>();
                    GameObject SelectedObj = UnitObj.transform.Find("Selected").gameObject;

                    //If not already in the dragged units
                    if (!UnitAlreadyInDraggedUnits(UnitObj) && !DragMeshTrigger.UnitAlreadyInTriggeredUnits(UnitObj) && DragMeshTrigger.NewTriggerEvent)
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

        //DragMeshTrigger       
        if (UserIsDragging && DragMeshTrigger.NewTriggerEvent)
        {
            DragMeshTrigger.NewSelectedUnits.Clear();
            //Check if units are still triggered this event
            for (int i = 0; i < DragMeshTrigger.SelectedUnits.Count; i++)
            {
                GameObject UnitObj = DragMeshTrigger.SelectedUnits[i] as GameObject;
                if (!DragMeshTrigger.UnitAlreadyInTriggeredUnits(UnitObj))
                {
                    DragMeshTrigger.SelectedUnits.RemoveAt(i);
                    UnitObj.transform.Find("Selected").gameObject.SetActive(false);
                }
                else
                {
                    DragMeshTrigger.NewSelectedUnits.Add(UnitObj);
                    UnitObj.transform.Find("Selected").gameObject.SetActive(true);
                }
            }
            //Check which new units are triggered this event
            for (int i = 0; i < DragMeshTrigger.TriggeredUnits.Count; i++)
            {
                GameObject UnitObj = DragMeshTrigger.TriggeredUnits[i] as GameObject;
                if (!DragMeshTrigger.UnitAlreadyInDragMesh(UnitObj))
                {
                    DragMeshTrigger.NewSelectedUnits.Add(UnitObj);
                    UnitObj.transform.Find("Selected").gameObject.SetActive(true);
                }
            }

            DragMeshTrigger.SelectedUnits = DragMeshTrigger.NewSelectedUnits;
            DragMeshTrigger.NewTriggerEvent = false;
        }
            DragMeshTrigger.TriggeredUnits.Clear();

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

    public float DistanceFromCameraToGround()
    {
        float extend = 50f;
        RaycastHit distance;
        float newFarPlaneValue = 0;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out distance, Mathf.Infinity, TerrainOnly))
        {
            DistanceToGround = Vector3.Distance(Camera.main.transform.position, distance.point);
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 1000, Color.green);
            newFarPlaneValue = DistanceToGround - Camera.main.nearClipPlane + extend;
        }
        return newFarPlaneValue;
    }

    public ClipPlainPoints CameraClipPlanePoints(float distance)
    {
        ClipPlainPoints clipPlainPoints = new ClipPlainPoints();

        Transform transform = Camera.main.transform;
        Vector3 pos = transform.position;
        float halfFOV = (Camera.main.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = Camera.main.aspect;

        float height = Mathf.Tan(halfFOV) * distance;
        float width = height * aspect;

        //Lower Right
        clipPlainPoints.LowerRight = pos + transform.forward * distance;
        clipPlainPoints.LowerRight += transform.right * width;
        clipPlainPoints.LowerRight -= transform.up * height;

        //Lower Left
        clipPlainPoints.LowerLeft = pos + transform.forward * distance;
        clipPlainPoints.LowerLeft -= transform.right * width;
        clipPlainPoints.LowerLeft -= transform.up * height;

        //Upper Right
        clipPlainPoints.UpperRight = pos + transform.forward * distance;
        clipPlainPoints.UpperRight += transform.right * width;
        clipPlainPoints.UpperRight += transform.up * height;

        //Upper Left
        clipPlainPoints.UpperLeft = pos + transform.forward * distance;
        clipPlainPoints.UpperLeft -= transform.right * width;
        clipPlainPoints.UpperLeft += transform.up * height;

        return clipPlainPoints;
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
                {
                    if (CurrentObject.GetComponent<Unit>() != null && CurrentObject.GetComponent<Structure>() == null)
                        CurrentObject.GetComponent<AIDestinationSetter>().target = hit.collider.gameObject.transform;
                    else
                    {
                        Debug.Log("Rallypont");
                        if (CurrentObject.transform == hit.collider.transform)
                        {
                            CurrentObject.GetComponent<Structure>().RallyTarget = null;
                            CurrentObject.GetComponent<Structure>().RallyPoint = CurrentObject.transform.position;
                            continue;
                        }
                        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
                        {
                            CurrentObject.GetComponent<Structure>().RallyPoint = Vector3.positiveInfinity;
                            CurrentObject.GetComponent<Structure>().RallyTarget = hit.collider.transform;
                        }
                        else 
                        {                           
                            CurrentObject.GetComponent<Structure>().RallyTarget = null;
                            CurrentObject.GetComponent<Structure>().RallyPoint = hit.point;
                        }
                    }           
                }

            }
        }
    }
  
    public static void SelectGatherTargets(RaycastHit hit)
    {
        if (CurrentlySelectedUnits.Count != 0)
        {
            for (int i = 0; i < CurrentlySelectedUnits.Count; i++)
            {
                GameObject CurrentObject = CurrentlySelectedUnits[i] as GameObject;
                if (CurrentObject != null && CurrentObject.GetComponent<Unit>().isGatherer)
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
                {
                    CurrentObject.GetComponent<AIDestinationSetter>().target = null;
                    CurrentObject.GetComponent<AIDestinationSetter>().ai.isStopped = false;
                }
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
        if (DragMeshTrigger.SelectedUnits.Count > 0)
        {
            for (int i = 0; i < DragMeshTrigger.SelectedUnits.Count; i++)
            {
                GameObject UnitObj = DragMeshTrigger.SelectedUnits[i] as GameObject;
                if (!UnitAlreadyInCurrentySelectedUnits(UnitObj))
                {
                    CurrentlySelectedUnits.Add(UnitObj);
                    UnitObj.GetComponent<Unit>().Selected = true;
                }
            }
            DragMeshTrigger.SelectedUnits.Clear();
        }
    }

    public void UpdateDragBoxMesh()
    {
        MeshCollider meshc = DragSelectMesh.GetComponent<MeshCollider>();
        meshc.sharedMesh = null;
        //Ratios
        Vector2 p0Ratio = new Vector2(
            BoxFinish.x / (Screen.width * 0.01f) * 0.01f,
            (BoxFinish.y + Mathf.Abs(BoxHeight)) / (Screen.height * 0.01f) * 0.01f
            );
        Vector2 p1Ratio = new Vector2(
            BoxStart.x / (Screen.width * 0.01f) * 0.01f,
            BoxStart.y / (Screen.height * 0.01f) * 0.01f
            );
        Vector2 p2Ratio = new Vector2(
            BoxStart.x / (Screen.width * 0.01f) * 0.01f,
            (BoxStart.y - Mathf.Abs(BoxHeight)) / (Screen.height * 0.01f) * 0.01f
            );
        Vector2 p3Ratio = new Vector2(
            BoxFinish.x / (Screen.width * 0.01f) * 0.01f,
            BoxFinish.y / (Screen.height * 0.01f) * 0.01f
            );

        ClipPlainPoints nearClipPlainPoints = CameraClipPlanePoints(Camera.main.nearClipPlane + 30f);
        ClipPlainPoints farClipPlainPoints = CameraClipPlanePoints(DistanceFromCameraToGround());

        float nearPlainWidth = Vector3.Distance(nearClipPlainPoints.LowerLeft, nearClipPlainPoints.LowerRight);
        float nearPlainHeight = Vector3.Distance(nearClipPlainPoints.UpperRight, nearClipPlainPoints.LowerRight);
        float farPlainWidth = Vector3.Distance(farClipPlainPoints.LowerLeft, farClipPlainPoints.LowerRight);
        float farPlainHeight = Vector3.Distance(farClipPlainPoints.UpperRight, farClipPlainPoints.LowerRight);

        Pointer.transform.position = nearClipPlainPoints.LowerLeft;
        Pointer.transform.eulerAngles = Camera.main.transform.eulerAngles;
        Pointer.transform.Translate(nearPlainWidth * p0Ratio.x, nearPlainHeight * p0Ratio.y, 0f);

        Vector3 p0 = Pointer.transform.position;

        Pointer.transform.position = nearClipPlainPoints.LowerLeft;
        Pointer.transform.Translate(nearPlainWidth * p1Ratio.x, nearPlainHeight * p1Ratio.y, 0f);

        Vector3 p1 = Pointer.transform.position;

        Pointer.transform.position = nearClipPlainPoints.LowerLeft;
        Pointer.transform.Translate(nearPlainWidth * p2Ratio.x, nearPlainHeight * p2Ratio.y, 0f);

        Vector3 p2 = Pointer.transform.position;

        Pointer.transform.position = nearClipPlainPoints.LowerLeft;
        Pointer.transform.Translate(nearPlainWidth * p3Ratio.x, nearPlainHeight * p3Ratio.y, 0f);

        Vector3 p3 = Pointer.transform.position;

        Pointer.transform.position = farClipPlainPoints.LowerLeft;
        Pointer.transform.Translate(farPlainWidth * p0Ratio.x, farPlainHeight * p0Ratio.y, 0f);

        Vector3 p4 = Pointer.transform.position;

        Pointer.transform.position = farClipPlainPoints.LowerLeft;
        Pointer.transform.Translate(farPlainWidth * p1Ratio.x, farPlainHeight * p1Ratio.y, 0f);

        Vector3 p5 = Pointer.transform.position;

        Pointer.transform.position = farClipPlainPoints.LowerLeft;
        Pointer.transform.Translate(farPlainWidth * p2Ratio.x, farPlainHeight * p2Ratio.y, 0f);

        Vector3 p6 = Pointer.transform.position;

        Pointer.transform.position = farClipPlainPoints.LowerLeft;
        Pointer.transform.Translate(farPlainWidth * p3Ratio.x, farPlainHeight * p3Ratio.y, 0f);

        Vector3 p7 = Pointer.transform.position;

        Mesh mesh = DragSelectMesh.GetComponent<MeshFilter>().mesh;

        #region Vertices
        Vector3[] vertices = new Vector3[]
       {
	        // Bottom
	        p0, p1, p2, p3,
 
	        // Left
	        p7, p4, p0, p3,
 
	        // Front
	        p4, p5, p1, p0,
 
	        // Back
	        p6, p7, p3, p2,
 
	        // Right
	        p5, p6, p2, p1,
 
	        // Top
	        p7, p6, p5, p4
       };
        #endregion

        mesh.vertices = vertices;
        meshc.sharedMesh = mesh;
        DragSelectMesh.transform.Translate(0.1f, 0f, 0f);
        DragSelectMesh.transform.Translate(-0.1f, 0f, 0f);
    }

    public void CreateDragBoxMesh()
    {
        DragSelectMesh = new GameObject();
        DragSelectMesh.name = "DragSelectMesh";
        DragSelectMesh.transform.position = Vector3.zero;
        DragSelectMesh.layer = LayerMask.NameToLayer("Mechanics");

        //MeshRenderer renderer = DragSelectMesh.AddComponent<MeshRenderer>();
        //renderer.material = DragSelectMeshMat;
        MeshFilter filter = DragSelectMesh.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        float length = 50f;
        float width = 50f;
        float height = 50f;

        #region Vertices
        Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
        Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
        Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
        Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
        Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f);
        Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
        Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);

        Vector3[] vertices = new Vector3[]
        {
	        // Bottom
	        p0, p1, p2, p3,
 
	        // Left
	        p7, p4, p0, p3,
 
	        // Front
	        p4, p5, p1, p0,
 
	        // Back
	        p6, p7, p3, p2,
 
	        // Right
	        p5, p6, p2, p1,
 
	        // Top
	        p7, p6, p5, p4
        };
        #endregion

        #region Normales
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normales = new Vector3[]
        {
	        // Bottom
	        down, down, down, down,
 
	        // Left
	        left, left, left, left,
 
	        // Front
	        front, front, front, front,
 
	        // Back
	        back, back, back, back,
 
	        // Right
	        right, right, right, right,
 
	        // Top
	        up, up, up, up
        };
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
	        // Bottom
	        _11, _01, _00, _10,
 
	        // Left
	        _11, _01, _00, _10,
 
	        // Front
	        _11, _01, _00, _10,
 
	        // Back
	        _11, _01, _00, _10,
 
	        // Right
	        _11, _01, _00, _10,
 
	        // Top
	        _11, _01, _00, _10,
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
	        // Bottom
	        3, 1, 0,
            3, 2, 1,			
 
	        // Left
	        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	        // Front
	        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	        // Back
	        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	        // Right
	        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	        // Top
	        3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
        };
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        DragSelectMesh.AddComponent<DragMeshTrigger>();
        Rigidbody rigidbody = DragSelectMesh.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        MeshCollider meshc = DragSelectMesh.AddComponent<MeshCollider>();
        meshc.convex = true;
        meshc.isTrigger = true;
        meshc.sharedMesh = filter.mesh;
        meshc.inflateMesh = true;

    }
}

#endregion
