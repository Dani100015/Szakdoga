using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class TechInfo : MonoBehaviour {

    public string techName;
    public string techInfo;
    public int researchTime;

    public int costPalladium;
    public int costIridiumm;
    public int costNullElement;

    public Image techIcon;

    Tech currentTech;
    Game game;

    List<Tech> playerTech = new List<Tech>();

    public Image currentProcessBar;
    float hitPoint;
    float maxHitPoint;

    GameObject techTipPanel;
    Image techTipPanelImage;
    Image processBacground;

    Vector3 offset;

    void Awake()
    {
        techTipPanel = GameObject.Find("TechTipPanel");
        game = GameObject.Find("Game").GetComponent<Game>();




    }
    void Start()
    {  
        playerTech = game.playerTechList;
        currentTech = Tech.techList.Find(x => x.Name == gameObject.name);

        processBacground = transform.Find("Background").GetComponent<Image>();
        currentProcessBar = transform.Find("Background").transform.Find("ProcessBar").GetComponent<Image>();

        techName = currentTech.Name;
        techInfo = currentTech.Description;
        researchTime = currentTech.ResearchTime;

        costPalladium = currentTech.PalladiumCost;
        costIridiumm = currentTech.IridiumCost;
        costNullElement = currentTech.EezoCost;

        techIcon = transform.Find("Image").GetComponent<Image>();      
         

        #region EventTrigger

        gameObject.AddComponent(typeof(EventTrigger));
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnMouseEnter((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { OnMouseExit((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnMouseDown((PointerEventData)data); });
        trigger.triggers.Add(entry);
        #endregion

        offset = new Vector3(150, -150, 0);
        techTipPanel.SetActive(false);
        processBacground.gameObject.SetActive(false);
    }
    void Update()
    {

        if (playerTech.Find(x => x.name == techIcon.transform.parent.name).Researched == false)
        {
            techIcon.color = Color.grey;
        }
        else
        {
            techIcon.color = Color.white;
        }

        if (hitPoint != 0)
        {
            UpdateProcessBar();
        }
    }

    public void OnMouseEnter(PointerEventData data)
    {
        techTipPanel.SetActive(true);
        techTipPanel.transform.position = Input.mousePosition + offset;

        //techTipPanelImage.sprite = techIcon.sprite;
        techTipPanel.transform.Find("TechInfo").GetComponent<Text>().text = techInfo;
        techTipPanel.transform.Find("TechTipPanelHead").transform.Find("TechName").GetComponent<Text>().text = techName;
        techTipPanel.transform.Find("TechTipPanelHead").transform.Find("TechIcon").GetComponent<Image>().sprite = techIcon.sprite;
    }
    public void OnMouseExit(PointerEventData data)
    {
        techTipPanel.SetActive(false);
    }
    public void OnMouseDown(PointerEventData data)
    {
        if (currentTech.Researched == false && 
            ((currentTech.Prerequisite == null) || (currentTech.Prerequisite.Researched == true)) &&
            Game.currentPlayer.Palladium - currentTech.PalladiumCost >= 0 && 
            Game.currentPlayer.Iridium - currentTech.IridiumCost >= 0 && 
            Game.currentPlayer.NullElement - currentTech.EezoCost >= 0)
            {
                Game.currentPlayer.Palladium -= currentTech.PalladiumCost;
                Game.currentPlayer.Iridium -= currentTech.IridiumCost;
                Game.currentPlayer.NullElement -= currentTech.EezoCost;

                hitPoint = 1 ;
                maxHitPoint= currentTech.ResearchTime;

                processBacground.gameObject.SetActive(true);
                StartCoroutine(WaitForResearched(currentTech));
            }
        
    }

    IEnumerator WaitForResearched(Tech currentTech)
    {
        while (hitPoint != maxHitPoint)
        {
            ResresearchProgress(1);
            yield return new WaitForSeconds(1);
        }

        currentTech.Researched = true;
        processBacground.gameObject.SetActive(false);

        hitPoint = 0;
        maxHitPoint = 0;
    }

    void UpdateProcessBar()
    {
        float ratio = hitPoint / maxHitPoint;
        currentProcessBar.GetComponent<RectTransform>().localScale = new Vector3(ratio, 1, 1);
    }

    void ResresearchProgress(float num)
    {
        hitPoint += num;
        if (hitPoint >  maxHitPoint)
        {
            hitPoint = maxHitPoint;
            Debug.Log("Research Completed!");
        }
    }
}
