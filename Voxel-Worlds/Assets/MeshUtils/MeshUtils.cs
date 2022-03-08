using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;
public class MeshUtils 
{

    public enum BlockSide
    {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    };

    
    public enum BlockType
    {
      GRASSTOP, GRASSSIDE, DIRT, WATER, STONE, SAND  
    };

    public static Vector2[,] blockUVs =
    {
        /* GRASSTOP */ 
        {new Vector2(0.125f,0.375f),
            new Vector2(0.1875f,0.375f),
            new Vector2(0.125f,0.4375f),
            new Vector2(0.1875f,0.4375f)},
        
        /* GRASSSIDE */ 
        {new Vector2(0.1875f,0.9375f), 
            new Vector2(0.25f,0.9375f),
            new Vector2(0.1875f,1f),
            new Vector2(0.25f,1f)},
        
        /* DIRT */ 
        {new Vector2(0.125f,0.9375f),
            new Vector2(0.1875f,0.9375f),
            new Vector2(0.125f,1f),
            new Vector2(0.1875f,1f)},
        
        /* WATER */ 
        {new Vector2(0.875f,0.125f),
            new Vector2(0.9375f,0.125f),
            new Vector2(0.875f,0.1875f),
            new Vector2(0.9375f,0.1875f)},
        
        /* STONE */ 
        {new Vector2(0,0.875f),
            new Vector2(0.0625f,0.875f),
            new Vector2(0,0.9375f),
            new Vector2(0.0625f,0.9375f)},
        
        /* SAND */ 
        {new Vector2(0.125f,0.875f),
            new Vector2(0.1875f,0.875f),
            new Vector2(0.125f,0.9375f),
            new Vector2(0.1875f,0.9375f)},
    };
    
    public static Mesh mergeMeshes(Mesh[] meshes)
    {
        Mesh mesh = new Mesh();
        // used hash to quickly access in the loop
        Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
        HashSet<VertexData> pointsHash = new HashSet<VertexData>();
        List<int> triangles = new List<int>();

        int pIndex = 0;
        
        // loop through each mesh
        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i] == null)
            {
                continue;
            }

            // loop through each vertex of the current mesh
            for (int j = 0; j < meshes[i].vertices.Length; j++)
            {
                Vector3 vertice = meshes[i].vertices[j];
                Vector3 normal = meshes[i].normals[j];
                Vector2 uvValue = meshes[i].uv[j];

                VertexData points = new VertexData(vertice, normal, uvValue);

                if (!pointsHash.Contains(points))
                {
                    pointsOrder.Add(points,pIndex);
                    pointsHash.Add(points);

                    pIndex++;
                }
            }

            for (int t = 0; t < meshes[i].triangles.Length; t++)
            {
                int triPoint = meshes[i].triangles[t];
                
                Vector3 vertice = meshes[i].vertices[triPoint];
                Vector3 normal = meshes[i].normals[triPoint];
                Vector2 uvValue = meshes[i].uv[triPoint];
                
                VertexData points = new VertexData(vertice, normal, uvValue);

                int index;
                pointsOrder.TryGetValue(points, out index);
                triangles.Add(index);
            }

            meshes[i] = null;
        }
        extractArrays(pointsOrder, mesh);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();

        return mesh;
    }


    public static void extractArrays(Dictionary<VertexData, int> list, Mesh mesh)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvValues = new List<Vector2>();

        foreach (var i in list.Keys)
        {
            vertices.Add(i.Item1);
            normals.Add(i.Item2);
            uvValues.Add(i.Item3);
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvValues.ToArray();
    }
  
}
