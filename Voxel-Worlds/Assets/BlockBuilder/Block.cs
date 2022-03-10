using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Mesh mesh;
    private Chunk parentChunk;
    public Block(Vector3 offset, MeshUtils.BlockType blockType, Chunk chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPos = offset - chunk.location;

        if (blockType != MeshUtils.BlockType.AIR)
        {
            List<Quad> quads = new List<Quad>();

            if (!HasSolidNeighbour((int) blockLocalPos.x, (int) blockLocalPos.y - 1, (int) blockLocalPos.z))
            {
                if (blockType == MeshUtils.BlockType.GRASSSIDE)
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, MeshUtils.BlockType.DIRT));
                }
                else
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, blockType));
                }
            }

            if (!HasSolidNeighbour((int) blockLocalPos.x, (int) blockLocalPos.y + 1, (int) blockLocalPos.z))
            {
                if (blockType == MeshUtils.BlockType.GRASSSIDE)
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, MeshUtils.BlockType.GRASSTOP));
                }
                else
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, blockType));
                }
                
            }

            if (!HasSolidNeighbour((int) blockLocalPos.x - 1, (int) blockLocalPos.y, (int) blockLocalPos.z))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.LEFT, offset, blockType));
            }

            if (!HasSolidNeighbour((int) blockLocalPos.x + 1, (int) blockLocalPos.y, (int) blockLocalPos.z))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.RIGHT, offset, blockType));
            }

            if (!HasSolidNeighbour((int) blockLocalPos.x, (int) blockLocalPos.y, (int) blockLocalPos.z + 1))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.FRONT, offset, blockType));
            }

            if (!HasSolidNeighbour((int) blockLocalPos.x, (int) blockLocalPos.y, (int) blockLocalPos.z - 1))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.BACK, offset, blockType));
            }

            if (quads.Count == 0)
            {
                return;
            }

            Mesh[] sideMeshes = new Mesh[quads.Count];
            int m = 0;
            foreach (var q in quads)
            {
                sideMeshes[m] = q.mesh;
                m++;
            }

            mesh = MeshUtils.mergeMeshes(sideMeshes);
            mesh.name = "Cube_0_0_0";
        }
    }

    public bool HasSolidNeighbour(int x, int y, int z)
    {
        if (x < 0 || x >= parentChunk.width || y < 0 || y >= parentChunk.height || z < 0 || z >= parentChunk.depth)
        {
            return false;
        }

        if (parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.AIR
            || parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.WATER)
        {
            return false;
        }

        return true;
    }
}
