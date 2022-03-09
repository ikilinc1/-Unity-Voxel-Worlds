using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float heightScale = 2.0f;
    public float scale = 0.5f;

    public int octaves = 1;

    public float heighOffset = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        Graph();
    }

    void Graph()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        
        int z = 11;
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        for (int x = 0; x < lineRenderer.positionCount; x++)
        {
            float y = MeshUtils.fBM(x,z,octaves,scale,heightScale,heighOffset);
            positions[x] = new Vector3(x, y, z);
        }
        lineRenderer.SetPositions(positions);
    }

    private void OnValidate()
    {
        Graph();
    }
}
