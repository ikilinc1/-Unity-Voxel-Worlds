using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Perlin3DGrapher : MonoBehaviour
{
    private Vector3 dimensions = new Vector3(10, 10, 10);
    
    public float heightScale = 2.0f;
    [Range(0.0f, 1.0f)]
    public float scale = 0.5f;
    public int octaves = 1;
    public float heighOffset = 1f;
    [Range(0.0f, 10.0f)]
    public float drawCutOff = 1.0f;

    void CreateCubes()
    {
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "perlin_cube";
                    cube.transform.parent = this.transform;
                    cube.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    void Graph()
    {
        // destroy existing cubes
        MeshRenderer[] cubes = this.GetComponentsInChildren<MeshRenderer>();
        if (cubes.Length == 0)
        {
            CreateCubes();
        }

        // neccessery because we run this in edit mode
        if (cubes.Length == 0)
        {
            return;
        }
        
        
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    float perlin3D = MeshUtils.fBM3D(x, y, z, octaves, scale, heightScale, heighOffset);
                    if (perlin3D < drawCutOff)
                    {
                        cubes[x + (int)dimensions.x * (y + (int)dimensions.z * z)].enabled = false;
                    }
                    else
                    {
                        cubes[x + (int)dimensions.x * (y + (int)dimensions.z * z)].enabled = true;
                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        Graph();
    }
}
