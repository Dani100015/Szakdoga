using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRingRenderer : MonoBehaviour {


    public List<GameObject> planets;

    public int segments;
    public float xradius;
    public float zradius;

    LineRenderer line;
    List<LineRenderer> lines;
    GameObject LineObject;

    Transform central;

    Color c1 = Color.cyan;
    Color c2 = Color.cyan;

    void Start()
    {
        GeneratePlanetLine();
        GenerateSystemBorderLine();
    }

    void CreatePoints(LineRenderer line)
    {
        float x;
        float y = 0;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * zradius;

            Vector3 offset = line.transform.parent.transform.position;

            line.SetPosition(i, new Vector3(x + offset.x, y, z + offset.z));

            angle += (360f / segments);
        }


    }

    void GeneratePlanetLine()
    {
        planets = new List<GameObject>();
        lines = new List<LineRenderer>();

        segments = 360;

        for (int i = 0; i < transform.childCount; i++)
        {
            planets.Add(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < planets.Count; i++)
        {
            LineObject = new GameObject();
            LineObject.AddComponent<LineRenderer>();

            xradius = planets[i].transform.position.x;
            zradius = planets[i].transform.position.z;

            float distance = Mathf.Sqrt((xradius * xradius) + (zradius * zradius));

            xradius = distance;
            zradius = distance;

            line = LineObject.GetComponent<LineRenderer>();
            line.startWidth = (0.5f);
            line.endWidth = (0.5f);

            line.material.color = Color.grey;
            //line.material.mainTextureOffset = new Vector2(100, 100);   
               
            line.positionCount = (segments + 1);
            line.useWorldSpace = false;
            line.gameObject.layer = 15;
            line.name = "RingLine";
            line.receiveShadows = false;

           
            LineObject.transform.SetParent(transform.parent.transform.Find("LineContainer"));
            lines.Add(line);

            CreatePoints(line);
           


        }
    }

    void GenerateSystemBorderLine()
    { 
        LineObject = new GameObject();
        LineObject.AddComponent<LineRenderer>();

        xradius = 200;
        zradius = 200;

        float distance = Mathf.Sqrt((xradius * xradius) + (zradius * zradius));

        xradius = distance;
        zradius = distance;

        line = LineObject.GetComponent<LineRenderer>();

        line.startWidth = (1f);
        line.endWidth = (1f);
    
        line.material.color = Color.cyan;
        line.material.mainTextureOffset = new Vector2(100, 100);

        line.positionCount = (segments + 1);

        line.useWorldSpace = false;
        line.gameObject.layer = 15;

        lines.Add(line);
        LineObject.transform.SetParent(transform.parent.transform.Find("LineContainer"));

        CreatePoints(line);

    }
}
