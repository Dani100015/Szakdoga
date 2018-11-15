using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pathfinding;
using System.Linq;
using System.Text;

public class GUISetup : MonoBehaviour
{
    public GameObject CommandContainer;
    public GameObject DetailContainer;
    public GameObject UnitListContainer;
    public GameObject ToolTipContainter;
    public static GameObject PlayerInfoContainer;
    public GameObject prefab;

    public GameObject Ghost;
    public static Material GhostMaterial;
    public static Material GhostMaterialRed;
    public static bool GhostActive = false;
    public static bool canBuildStructure;
    int CurrentGhost;
    Canvas canvas;


    public static bool PassedTriggerTest;

    public void BuildClick()
    {
        CommandContainer.transform.Find("Movement").gameObject.SetActive(false);
        Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().ShowBuildables = true;
    }

    void Start()
    {
        GhostMaterial = Resources.Load("Materials/GhostMaterial", typeof(Material)) as Material;
        GhostMaterialRed = Resources.Load("Materials/GhostMaterialRed", typeof(Material)) as Material;
        PlayerInfoContainer = GameObject.Find("GameCanvas").transform.Find("PlayerInfoBar").gameObject;
        canvas = GameObject.Find("GameCanvas").GetComponent<Canvas>();
        UpdatePlayerInfoBar();

    }

    /// <summary>
    /// Ellenőrzi, van-e elég nyersanyag az egység kiképzéséhez, illetve hely a flottában
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    bool CostCheck(Unit unit)
    {
        if (Game.currentPlayer.iridium - unit.iridiumCost >= 0 &&
            Game.currentPlayer.palladium - unit.palladiumCost >= 0 &&
            Game.currentPlayer.nullElement - unit.eezoCost >= 0 && Game.currentPlayer.CurrentPopulation + unit.PopulationCost <= Game.currentPlayer.MaxPopulation)
        {
            Game.currentPlayer.iridium -= unit.iridiumCost;
            Game.currentPlayer.palladium -= unit.palladiumCost;
            Game.currentPlayer.nullElement -= unit.eezoCost;
            Game.currentPlayer.CurrentPopulation += unit.PopulationCost;

            UpdatePlayerInfoBar();

            return true;
        }
        return false;
    }

    bool BuildCostCheck(Structure building)
    {
        if (Game.currentPlayer.iridium - building.iridiumCost >= 0 &&
            Game.currentPlayer.palladium - building.palladiumCost >= 0 &&
            Game.currentPlayer.nullElement - building.eezoCost >= 0)
            return true;
        return false;
    }

    void BuildCost(Structure building)
    {
        Game.currentPlayer.iridium -= building.iridiumCost;
        Game.currentPlayer.palladium -= building.palladiumCost;
        Game.currentPlayer.nullElement -= building.eezoCost;

        UpdatePlayerInfoBar();
    }

    public void UpdatePlayerInfoBar()
    {
        PlayerInfoContainer.transform.Find("TextIridium").GetComponent<Text>().text = "Iridium: " + Game.currentPlayer.iridium;
        PlayerInfoContainer.transform.Find("TextPalladium").GetComponent<Text>().text = "Palladium: " + Game.currentPlayer.palladium;
        PlayerInfoContainer.transform.Find("TextNullElement").GetComponent<Text>().text = "Null elem: " + Game.currentPlayer.nullElement;
        PlayerInfoContainer.transform.Find("TextPopulation").GetComponent<Text>().text = "Pop: " + Game.currentPlayer.CurrentPopulation + "/" + Game.currentPlayer.MaxPopulation;
    }

    void OnGUI()
    {
        if (Mouse.CurrentlyFocusedUnit != null)
        {
            #region Építkezés
            if (Mouse.CurrentlyFocusedUnit.GetComponent<Unit>() != null && Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().isGatherer && Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().ShowBuildables)
            {
                int offset = (int)(48 * canvas.scaleFactor * 1.6);
                int j = 0;
                RectTransform container = CommandContainer.transform as RectTransform;
                for (int i = 0; i < Game.currentPlayer.BuildableUnits.Count; i++)
                {
                    GameObject unit = Game.currentPlayer.BuildableUnits[i];
                    Structure unitObj = unit.GetComponent<Structure>();
                    GUIContent content = new GUIContent("", unit.name);
                    if (unit.GetComponent<Structure>() == null)
                        continue;

                    GUIStyle Icon = new GUIStyle();
                    Icon.normal.background = unitObj.MenuIcon;
                    Icon.hover.background = unitObj.MenuIconRo;

                    Rect currentButtonPosition = new Rect(Screen.width * .805f + (offset * (j % 4)), Screen.height * .83f + (offset * (int)(j / 4)), 46 * canvas.scaleFactor * 1.5f, 39 * canvas.scaleFactor * 1.5f);

                    if (GUI.Button(currentButtonPosition, content, Icon))
                    {
                        if (BuildCostCheck(unitObj))
                        {
                            if (GhostActive)
                                Destroy(Ghost.gameObject);
                            Ghost = Instantiate(unit.GetComponent<Structure>().GUIGhost, Vector3.zero, Quaternion.identity) as GameObject;
                            CurrentGhost = i;
                            Ghost.GetComponent<Renderer>().material = GhostMaterial;
                            Ghost.AddComponent<UnitGhost>();
                            Ghost.AddComponent<GhostTestTrigger>();
                            Ghost.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                            Ghost.GetComponent<Renderer>().receiveShadows = false;
                            Ghost.name = unit.name;
                            Ghost.transform.rotation = unit.GetComponent<Structure>().GUIGhost.transform.rotation;
                            GhostActive = true;

                            Rigidbody body = Ghost.AddComponent<Rigidbody>();
                            body.useGravity = false;
                            body.isKinematic = true;

                            BoxCollider box = Ghost.AddComponent<BoxCollider>();
                            box.isTrigger = true;
                        }
                    }
                    j++;
                }

                if (GUI.tooltip != "")
                {
                    GameObject unit = Game.currentPlayer.BuildableUnits.Where(x => x.name == GUI.tooltip).SingleOrDefault();
                    Unit unitObj = unit.GetComponent<Unit>();
                    ToolTipContainter.SetActive(true);
                    ToolTipContainter.transform.Find("TextName").GetComponent<Text>().text = unit.name;

                    //Egysék költségek
                    StringBuilder sb = new StringBuilder();
                    if (unitObj.iridiumCost != 0)
                        sb.Append("Iridium: " + unitObj.iridiumCost);
                    if (unitObj.palladiumCost != 0)
                        sb.Append("\r\nPalladium: " + unitObj.palladiumCost);
                    if (unitObj.eezoCost != 0)
                        sb.Append("\r\nEezo: " + unitObj.eezoCost);

                    ToolTipContainter.transform.Find("TextInfo").GetComponent<Text>().text = sb.ToString();
                }
                else ToolTipContainter.SetActive(false);
                j = 7;
                if (GUI.Button(new Rect(Screen.width * .95f, Screen.height * .85f + (offset * (int)(j / 4)), 46 * canvas.scaleFactor * 1.5f, 39 * canvas.scaleFactor * 1.5f), "<--"))
                {
                    CommandContainer.transform.Find("Movement").gameObject.SetActive(true);
                    Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().ShowBuildables = false;
                }
            }
            #endregion

            #region Kiképzés
            if (Mouse.CurrentlyFocusedUnit != null && !Mouse.CurrentlyFocusedUnit.tag.Equals("PlaceHolder") && Mouse.CurrentlyFocusedUnit.GetComponent<Structure>() != null && Mouse.CurrentlyFocusedUnit.GetComponent<Structure>().TrainableUnits.Count != 0)
            {
                Structure currentStructure = Mouse.CurrentlyFocusedUnit.GetComponent<Structure>();
                int offset = (int)(48 * canvas.scaleFactor * 1.6);
                int j = 0;

                for (int i = 0; i < currentStructure.TrainableUnits.Count; i++)
                {
                    GameObject unit = currentStructure.TrainableUnits[i];
                    Unit unitObj = unit.GetComponent<Unit>();
                    GUIContent content = new GUIContent("", unit.name);

                    GUIStyle Icon = new GUIStyle();
                    Icon.normal.background = unitObj.MenuIcon;
                    Icon.hover.background = unitObj.MenuIconRo;
                    RectTransform container = CommandContainer.transform as RectTransform;


                    Rect currentButtonPosition = new Rect(Screen.width * .805f + (offset * (j % 4)), Screen.height * .83f + (offset * (int)(j / 4)), 46 * canvas.scaleFactor * 1.5f, 39 * canvas.scaleFactor * 1.5f);
                    if (GUI.Button(currentButtonPosition, content, Icon))
                    {

                        if (currentStructure.TrainingQueue.Count < 7)
                        {
                            if (CostCheck(unitObj))
                            {
                                currentStructure.TrainingQueue.Add(Game.currentPlayer.BuildableUnits.Where(x => x.name == currentStructure.TrainableUnits[i].name).SingleOrDefault());
                                if (currentStructure.TrainingQueue.Count == 1)
                                    currentStructure.StartCoroutine("Train");
                            }
                        }
                    }
                    j++;
                }

                //Egység információk
                if (GUI.tooltip != "")
                {
                    GameObject unit = Game.currentPlayer.BuildableUnits.Where(x => x.name == GUI.tooltip).SingleOrDefault();
                    Unit unitObj = unit.GetComponent<Unit>();
                    ToolTipContainter.SetActive(true);
                    ToolTipContainter.transform.Find("TextName").GetComponent<Text>().text = unit.name;

                    //Egysék költségek
                    StringBuilder sb = new StringBuilder();
                    if (unitObj.iridiumCost != 0)
                        sb.Append("Iridium: " + unitObj.iridiumCost);
                    if (unitObj.palladiumCost != 0)
                        sb.Append("\r\nPalladium: " + unitObj.palladiumCost);
                    if (unitObj.eezoCost != 0)
                        sb.Append("\r\nEezo: " + unitObj.eezoCost);

                    ToolTipContainter.transform.Find("TextInfo").GetComponent<Text>().text = sb.ToString();
                }
                else ToolTipContainter.SetActive(false);

                j = 0;
                if (currentStructure.TrainingQueue.Count != 0)
                {
                    DetailContainer.transform.Find("UnitArmor").gameObject.SetActive(false);
                }
                for (int i = 0; i < currentStructure.TrainingQueue.Count; i++)
                {
                    GameObject unit = ((currentStructure.TrainingQueue).ToArray())[i] as GameObject;
                    Unit unitObj = unit.GetComponent<Unit>();

                    GUIStyle Icon = new GUIStyle();
                    Icon.normal.background = unitObj.MenuIcon;
                    Icon.hover.background = unitObj.MenuIconRo;
                    offset = (int)(48 * canvas.scaleFactor * 1.55);
                    if (GUI.Button(new Rect(Screen.width * .065f + (offset * (j % 4)), Screen.height * .85f + (offset * (int)(j / 4)), 46 * canvas.scaleFactor * 1.5f, 39 * canvas.scaleFactor * 1.5f), "", Icon))
                    {
                        Game.currentPlayer.iridium += unitObj.iridiumCost;
                        Game.currentPlayer.palladium += unitObj.palladiumCost;
                        Game.currentPlayer.nullElement += unitObj.eezoCost;
                        if (i == 0)
                        {
                            currentStructure.StopCoroutine("Train");
                            currentStructure.StartCoroutine("Train");
                        }

                        currentStructure.TrainingQueue.RemoveAt(i);
                        if (currentStructure.TrainingQueue.Count == 0)
                        {
                            currentStructure.StopCoroutine("Train");
                            DetailContainer.transform.Find("UnitArmor").gameObject.SetActive(true);

                        }
                    }
                    j++;
                }

            }
            #endregion

            #region UnitDetails
            DetailContainer.SetActive(true);
            DetailContainer.transform.Find("UnitName").GetComponent<Text>().text = Mouse.CurrentlyFocusedUnit.name;
            DetailContainer.transform.Find("Icon").transform.Find("UnitHP").GetComponent<Text>().text = Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().maxHealth + " / " + Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().currentHealth;
            DetailContainer.transform.Find("UnitArmor").GetComponent<Text>().text = "Armor: " + Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().Armor;

            if (Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().attackDamage != 0)
            {
                DetailContainer.transform.Find("UnitAttack").GetComponent<Text>().text = "Attack: " + Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().attackDamage;
                DetailContainer.transform.Find("UnitAttackSpeed").GetComponent<Text>().text = "Attack Speed: " + Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().attackSpeed;
                DetailContainer.transform.Find("UnitRange").GetComponent<Text>().text = "Range: " + Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().Range;
            }
            else
            {
                DetailContainer.transform.Find("UnitAttack").GetComponent<Text>().text = "";
                DetailContainer.transform.Find("UnitAttackSpeed").GetComponent<Text>().text = "";
                DetailContainer.transform.Find("UnitRange").GetComponent<Text>().text = "";
            }

            if (Mouse.CurrentlyFocusedUnit.GetComponent<AIPath>() != null)
                DetailContainer.transform.Find("UnitSpeed").GetComponent<Text>().text = "Speed: " + Mouse.CurrentlyFocusedUnit.GetComponent<AIPath>().maxSpeed;
            else DetailContainer.transform.Find("UnitSpeed").GetComponent<Text>().text = "";

            #endregion

            #region CommandCard
            if (Mouse.CurrentlyFocusedUnit.GetComponent<Unit>() != null && !Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().ShowBuildables && !Mouse.CurrentlyFocusedUnit.GetComponent<Structure>())
            {
                CommandContainer.transform.Find("Movement").gameObject.SetActive(true);
                if (!Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().isGatherer)
                {
                    CommandContainer.transform.Find("Movement").Find("RepairButton").gameObject.SetActive(false);
                    CommandContainer.transform.Find("Movement").Find("BuildButton").gameObject.SetActive(false);
                }
                else
                {
                    CommandContainer.transform.Find("Movement").Find("RepairButton").gameObject.SetActive(true);
                    CommandContainer.transform.Find("Movement").Find("BuildButton").gameObject.SetActive(true);
                }
            }
            else CommandContainer.transform.Find("Movement").gameObject.SetActive(false);
            #endregion

            while (UnitListContainer.transform.childCount < Mouse.CurrentlySelectedUnits.Count)
            {
                GameObject scrollItemObject = Instantiate(prefab, UnitListContainer.transform);
            }
        }
        else
        {
            DetailContainer.SetActive(false);
            CommandContainer.transform.Find("Movement").gameObject.SetActive(false);
            DeleteIcons();
        }

    }

    public void DeleteIcons()
    {
        var children = new List<GameObject>();
        foreach (Transform child in UnitListContainer.transform)
            children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }

    void LateUpdate()
    {
        if (GhostActive)
        {
            if (PassedTriggerTest)
            {
                Ghost.GetComponent<Renderer>().material = GhostMaterial;
                canBuildStructure = true;
            }
            else
            {
                Ghost.GetComponent<Renderer>().material = GhostMaterialRed;
                canBuildStructure = false;
            }

            if (Input.GetMouseButtonUp(0) && canBuildStructure && !(EventSystem.current.IsPointerOverGameObject()))
            {
                //Épület létrehozása  
                Unit builder = Mouse.CurrentlyFocusedUnit.GetComponent<Unit>();
                builder.CurrentlyBuiltObject = Game.currentPlayer.BuildableUnits[CurrentGhost];
                BuildCost(Game.currentPlayer.BuildableUnits[CurrentGhost].GetComponent<Structure>());
                builder.gameObject.GetComponent<AIDestinationSetter>().ai.destination = new Vector3(Mouse.currentMousePoint.x, 5f, Mouse.currentMousePoint.z);
                builder.gameObject.GetComponent<AIDestinationSetter>().ai.isStopped = false;
                builder.StartCoroutine("Build");
                GhostActive = false;
                Destroy(Ghost.gameObject);
            }
        }
    }
}
