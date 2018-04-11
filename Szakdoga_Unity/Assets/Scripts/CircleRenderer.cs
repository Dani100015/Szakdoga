using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour {

    public int vertexCount = 40;
    public float lineWidth = 0.2f;
    public float radius;
    public bool circleFillScreen;

    LineRenderer lineRenderer;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupCircleRendrer();
    }

    private void SetupCircleRendrer()
    {
        lineRenderer.widthMultiplier = lineWidth;

        if (circleFillScreen)
        {
            radius = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelRect.yMax, 0f)),
                                      Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelRect.yMin, 0f))) 
                                      * 0.5f - lineWidth;
        }

        float deltaTheta = (float)(2f * Math.PI) / vertexCount;
        float theta = 0f;

        lineRenderer.positionCount = vertexCount;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta)) + transform.parent.position;
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    private void OnDrawGizmos()
    {
        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;

        Vector3 oldPos = transform.parent.position;
        for (int i = 0; i < vertexCount + 1; i++)
        {
            Vector3 newPos = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
            if (i != 0)
            {
                Gizmos.DrawLine(oldPos, transform.parent.position + newPos);
            }          
            oldPos = transform.parent.position + newPos;
            theta += deltaTheta;
        }
    }
}
