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
    public GameObject BuildingContainer;
    public GameObject prefab;


    void OnGUI()
    {    
        if (Mouse.CurrentlyFocusedUnit != null)
        {
            if (Mouse.CurrentlyFocusedUnit.GetComponent<Unit>() != null && Mouse.CurrentlyFocusedUnit.GetComponent<Unit>().isGatherer)
            {
                GUILayout.BeginArea(new Rect(-Screen.width - CommandContainer.transform.position.x,
                                             Screen.height- CommandContainer.transform.position.y, 100, 100));
                for (int i = 0; i < Game.currentPlayer.UnitNames.Count; i++)
                {
                    GUILayout.Button("Teszt");
                }
                GUILayout.EndArea();
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
            if (Mouse.CurrentlyFocusedUnit.GetComponent<Unit>() != null)
                CommandContainer.transform.Find("Movement").gameObject.SetActive(true);
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
