using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh;
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "ScriptedQuad";

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvValues = new Vector2[4];
        int[] triangles = new int[6];

        Vector2 uv00 = new Vector2(0, 0);
        Vector2 uv10 = new Vector2(1, 0);
        Vector2 uv01 = new Vector2(0, 1);
        Vector2 uv11 = new Vector2(1, 1);

        Vector3 point0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 point1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 point2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 point3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 point4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 point5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 point6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 point7 = new Vector3(-0.5f, 0.5f, -0.5f);

        vertices = new Vector3[] {point4, point5, point1, point0};
        normals = new Vector3[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward};
        uvValues = new Vector2[] {uv11, uv01, uv00, uv10};
        // should specify triangle points in clockwise
        triangles = new[] {3,1,0,3,2,1};

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvValues;
        mesh.triangles = triangles;
        
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
