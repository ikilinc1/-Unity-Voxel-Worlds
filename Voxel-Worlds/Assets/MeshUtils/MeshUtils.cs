using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2, UnityEngine.Vector2>;
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

    public static int[] blockTypeHealth =
    {
        2, 2, 2, 1, 4, 1, 4, -1, 3, 4, -1, -1, -1, -1, -1, -1
    };

    public static HashSet<BlockType> canDrop = new HashSet<BlockType>
    {
        BlockType.SAND, BlockType.WATER
    };
    
    public static HashSet<BlockType> canFlow = new HashSet<BlockType>
    {
        BlockType.WATER
    };

    public enum BlockType {
        GRASSTOP, GRASSSIDE, DIRT, WATER, STONE, SAND, GOLD, BEDROCK, REDSTONE, DIAMOND, NOCRACK,
        CRACK1, CRACK2, CRACK3, CRACK4, AIR
    };
    
    public static Vector2[,] blockUVs = {
        /*GRASSTOP*/ {  new Vector2(0.125f, 0.375f), new Vector2(0.1875f,0.375f),
                        new Vector2(0.125f, 0.4375f), new Vector2(0.1875f,0.4375f) },
        /*GRASSSIDE*/ { new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                        new Vector2( 0.1875f, 1.0f ),new Vector2( 0.25f, 1.0f )},
        /*DIRT*/      { new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                        new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
        /*WATER*/     { new Vector2(0.875f,0.125f),  new Vector2(0.9375f,0.125f),
                        new Vector2(0.875f,0.1875f), new Vector2(0.9375f,0.1875f)},
        /*STONE*/     { new Vector2( 0, 0.875f ), new Vector2( 0.0625f, 0.875f),
                        new Vector2( 0, 0.9375f ),new Vector2( 0.0625f, 0.9375f )},
        /*SAND*/      { new Vector2(0.125f,0.875f),  new Vector2(0.1875f,0.875f),
                        new Vector2(0.125f,0.9375f), new Vector2(0.1875f,0.9375f)},
        /*GOLD*/        { new Vector2(0f,0.8125f),  new Vector2(0.0625f,0.8125f),
                          new Vector2(0f,0.875f), new Vector2(0.0625f,0.875f)},
        /*BEDROCK*/     {new Vector2( 0.3125f, 0.8125f ), new Vector2( 0.375f, 0.8125f),
                                new Vector2( 0.3125f, 0.875f ),new Vector2( 0.375f, 0.875f )},
        /*REDSTONE*/    {new Vector2( 0.1875f, 0.75f ), new Vector2( 0.25f, 0.75f),
                                new Vector2( 0.1875f, 0.8125f ),new Vector2( 0.25f, 0.8125f )},
        /*DIAMOND*/     {new Vector2( 0.125f, 0.75f ), new Vector2( 0.1875f, 0.75f),
                                new Vector2( 0.125f, 0.8125f ),new Vector2( 0.1875f, 0.8125f )},
        /*NOCRACK*/     {new Vector2( 0.6875f, 0f ), new Vector2( 0.75f, 0f),
                                new Vector2( 0.6875f, 0.0625f ),new Vector2( 0.75f, 0.0625f )},
        /*CRACK1*/      { new Vector2(0f,0f),  new Vector2(0.0625f,0f),
                                 new Vector2(0f,0.0625f), new Vector2(0.0625f,0.0625f)},
        /*CRACK2*/      { new Vector2(0.0625f,0f),  new Vector2(0.125f,0f),
                                 new Vector2(0.0625f,0.0625f), new Vector2(0.125f,0.0625f)},
        /*CRACK3*/      { new Vector2(0.125f,0f),  new Vector2(0.1875f,0f),
                                 new Vector2(0.125f,0.0625f), new Vector2(0.1875f,0.0625f)},
        /*CRACK4*/      { new Vector2(0.1875f,0f),  new Vector2(0.25f,0f),
                                 new Vector2(0.1875f,0.0625f), new Vector2(0.25f,0.0625f)}
    };
    
    public static float fBM(float x, float z ,int octaves, float scale, float heightScale, float heightOffset)
    {
        float total = 0;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * scale * frequency, z * scale * frequency) * heightScale;
            frequency *= 2;
        }

        return total + heightOffset;
    }
    
    public static float fBM3D(float x,float y, float z ,int octaves, float scale, float heightScale, float heightOffset)
    {
        float xy = fBM(x, y, octaves, scale, heightScale, heightOffset);
        float xz = fBM(x, z, octaves, scale, heightScale, heightOffset);
        float yx = fBM(y, x, octaves, scale, heightScale, heightOffset);
        float yz = fBM(y, z, octaves, scale, heightScale, heightOffset);
        float zx = fBM(z, x, octaves, scale, heightScale, heightOffset);
        float zy = fBM(z, y, octaves, scale, heightScale, heightOffset);

        return (xy + xz + yx + yz + zx + zy) / 6.0f;
    }
    
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
                Vector2 uvValueCracks = meshes[i].uv2[j];

                VertexData points = new VertexData(vertice, normal, uvValue, uvValueCracks);

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
                Vector2 uvValueCracks = meshes[i].uv2[triPoint];
                
                VertexData points = new VertexData(vertice, normal, uvValue, uvValueCracks);

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
        List<Vector2> uvValuesCracks = new List<Vector2>();

        foreach (var i in list.Keys)
        {
            vertices.Add(i.Item1);
            normals.Add(i.Item2);
            uvValues.Add(i.Item3);
            uvValuesCracks.Add(i.Item4);
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvValues.ToArray();
        mesh.uv2 = uvValuesCracks.ToArray();
    }
  
}
