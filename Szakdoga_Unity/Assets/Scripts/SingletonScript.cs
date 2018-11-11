using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScript : MonoBehaviour {

    void Awake()
    {
        
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

}
