using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour {

    public enum resourceType
    {
        Palladium,
        Iridium,
        ElementZero
    };

    public int Capacity;
    public resourceType Type;

}
