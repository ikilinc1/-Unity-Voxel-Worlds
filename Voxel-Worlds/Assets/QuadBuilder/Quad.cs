using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad
{
    public Mesh mesh;
    public Quad(Block.BlockSide side, Vector3 offset, MeshUtils.BlockType blockType)
    {
        mesh = new Mesh();
        mesh.name = "ScriptedQuad";

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvValues = new Vector2[4];
        int[] triangles = new int[6];

        Vector2 uv00 = MeshUtils.blockUVs[(int)blockType, 0];
        Vector2 uv10 = MeshUtils.blockUVs[(int)blockType, 1];
        Vector2 uv01 = MeshUtils.blockUVs[(int)blockType, 2];
        Vector2 uv11 = MeshUtils.blockUVs[(int)blockType, 3];

        Vector3 point0 = new Vector3(-0.5f, -0.5f, 0.5f) + offset;
        Vector3 point1 = new Vector3(0.5f, -0.5f, 0.5f) + offset;
        Vector3 point2 = new Vector3(0.5f, -0.5f, -0.5f) + offset;
        Vector3 point3 = new Vector3(-0.5f, -0.5f, -0.5f) + offset;
        Vector3 point4 = new Vector3(-0.5f, 0.5f, 0.5f) + offset;
        Vector3 point5 = new Vector3(0.5f, 0.5f, 0.5f) + offset;
        Vector3 point6 = new Vector3(0.5f, 0.5f, -0.5f) + offset;
        Vector3 point7 = new Vector3(-0.5f, 0.5f, -0.5f) + offset;

        // should specify triangle points in clockwise
        triangles = new[] {3,1,0,3,2,1};
        
        switch (side)
        {
            case Block.BlockSide.FRONT:
            {
                vertices = new Vector3[] {point4, point5, point1, point0};
                normals = new Vector3[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward};
                uvValues = new Vector2[] {uv11, uv01, uv00, uv10};
                break;
            }
            case Block.BlockSide.BACK:
            {
                vertices = new Vector3[] {point6, point7, point3, point2};
                normals = new Vector3[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back};
                uvValues = new Vector2[] {uv11, uv01, uv00, uv10};
                break;
            }
            case Block.BlockSide.LEFT:
            {
                vertices = new Vector3[] {point7, point4, point0, point3};
                normals = new Vector3[] {Vector3.left, Vector3.left, Vector3.left, Vector3.left};
                uvValues = new Vector2[] {uv11, uv01, uv00, uv10};
                break;
            }
            case Block.BlockSide.RIGHT:
            {
                vertices = new Vector3[] {point5, point6, point2, point1};
                normals = new Vector3[] {Vector3.right, Vector3.right, Vector3.right, Vector3.right};
                uvValues = new Vector2[] {uv11, uv01, uv00, uv10};
                break;
            }
            case Block.BlockSide.TOP:
            {
                vertices = new Vector3[] {point7, point6, point5, point4};
                normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
                uvValues = new Vector2[] {uv11, uv01, uv00, uv10};
                break;
            }
            case Block.BlockSide.BOTTOM:
            {
                vertices = new Vector3[] {point0, point1, point2, point3};
                normals = new Vector3[] {Vector3.down, Vector3.down, Vector3.down, Vector3.down};
                uvValues = new Vector2[] {uv11, uv01, uv00, uv10};
                break;
            }
            default:
            {
                break;
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvValues;
        mesh.triangles = triangles;
        
        mesh.RecalculateBounds();
    }
}
