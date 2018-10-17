using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollScript : MonoBehaviour {

	public ScrollRect scrollView;
    public GameObject scrollContent;
    public GameObject scrollItemPrefab;

	void Start () {
        for (int i = 0; i < 10; i++)
        {
            GenerateItems(i);
        }
        //scrollView.verticalNormalizedPosition = 1;
	}
	
	
	void Update () {
		
	}

    void GenerateItems(int itemNumber)
    {
        GameObject scrollItemObject = Instantiate(scrollItemPrefab);
        scrollItemObject.transform.SetParent(scrollContent.transform,false);
        //scrollItemObject.transform.Find("Item").gameObject.GetComponent<Text>().text = itemNumber.ToString(); 
    }
}
