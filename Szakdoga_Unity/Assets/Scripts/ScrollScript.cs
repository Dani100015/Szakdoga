using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class ScrollScript : MonoBehaviour {

	public ScrollRect scrollView;
    public GameObject scrollContent;
    public GameObject scrollItemPrefab;

    public Player currentPlayer;

    List<GameObject> UnitItemCount = new List<GameObject>();
    public Game game;
	void Start () {

        game = GameObject.Find("Game").GetComponent<Game>();
        currentPlayer = Game.currentPlayer;

        //scrollView.verticalNormalizedPosition = 1;
    }
	
	
	void Update () {
        //InvokeRepeating("GenerateItems", 1, 2);
    }

    void GenerateItems()
    {
        if (UnitItemCount.Count != currentPlayer.units.Count)
        {
            GameObject playerUnitItem;

            UnitItemCount = currentPlayer.units;
            for (int i = 0; i < UnitItemCount.Count; i++)
            {
                playerUnitItem = Instantiate(scrollItemPrefab);
                playerUnitItem.transform.SetParent(scrollContent.transform, false);
                playerUnitItem.transform.Find("ItemText").GetComponent<TextMeshProUGUI>().text = UnitItemCount[i].gameObject.name;
                playerUnitItem.GetComponent<GUI_ImperialInfoPanel_UnitDetail>().unitObject = UnitItemCount[i].gameObject;
            }
        }

    }
}
