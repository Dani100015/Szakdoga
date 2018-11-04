using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISetup : MonoBehaviour
{
    public GameObject CommandContainer;
    public GameObject DetailContainer;
    public GameObject UnitListContainer;
    public GameObject prefab;

    public GameObject Ghost;
    public Material GhostMaterial;
    public static Material GhostMaterialRed;
    public static bool GhostActive = false;
    public static bool canBuildStructure;

    public void BuildClick()
    {
        CommandContainer.transform.Find("Movement").gameObject.SetActive(false);
        Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().ShowBuildables = true;
    }

    void OnGUI()
    {    
        if (Mouse.CurrentlyFocusedUnit != null)
        {
            if (Mouse.CurrentlyFocusedUnit.GetComponent<Unit>() != null && Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().isGatherer && Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().ShowBuildables)
            {
                int offset = 48;
                int j = 0;
                for (int i = 0; i < Game.currentPlayer.UnitNames.Count; i++)
                {
                    GameObject unit = Resources.Load(Game.currentPlayer.UnitPaths[i], typeof(GameObject)) as GameObject;
                    if (unit.GetComponent<Structure>() == null)
                        continue;

                    GUIStyle Icon = new GUIStyle();
                    Icon.normal.background = Game.currentPlayer.UnitIcons[i];
                    Icon.hover.background = Game.currentPlayer.UnitIconsRo[i];      

                    if (GUI.Button(new Rect(Screen.width - 205 + (offset*(j%4)),Screen.height-100 + (offset*(int)(j/4)), 46, 39),"",Icon))
                    {
                        if (GhostActive)
                            Destroy(Ghost.gameObject);
                        Ghost = Instantiate(unit.GetComponent<Structure>().GUIGhost,Vector3.zero,Quaternion.identity) as GameObject;
                        Ghost.GetComponent<Renderer>().material = GhostMaterial;
                        Ghost.AddComponent<UnitGhost>();
                        Ghost.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        Ghost.GetComponent<Renderer>().receiveShadows = false;
                        Ghost.name = Game.currentPlayer.UnitNames[i];
                        Ghost.transform.rotation = unit.GetComponent<Structure>().GUIGhost.transform.rotation;
                        GhostActive = true;
                    }
                    j++;
                }
                j = 7;
                if (GUI.Button(new Rect(Screen.width - 205 + (offset * (j % 4)), Screen.height - 100 + (offset * (int)(j / 4)), 46, 39), "<--"))
                {
                    CommandContainer.transform.Find("Movement").gameObject.SetActive(true);
                    Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().ShowBuildables = false;
                }
            }

            #region UnitDetails
            DetailContainer.SetActive(true);
            DetailContainer.transform.Find("UnitName").GetComponent<Text>().text = Mouse.CurrentlyFocusedUnit.name;
            DetailContainer.transform.Find("Icon").transform.Find("UnitHP").GetComponent<Text>().text = Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().maxHealth + " / " + Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().currentHealth;
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
            if (Mouse.CurrentlyFocusedUnit.GetComponent<Unit>() != null && !Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().ShowBuildables)
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
            var children = new List<GameObject>();
            foreach (Transform child in UnitListContainer.transform)
                children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));
        }  
    
    }

}
