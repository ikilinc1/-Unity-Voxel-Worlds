using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class TreeCreator : MonoBehaviour
{
    public Vector3 dimensions = new Vector3(3, 6, 3);
    public GameObject[,,] allCubes;
    public string blockDetails = "";
    int halfX;
    int halfZ;

    void CreateCubes()
    {
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = x+"|"+y+"|"+z;
                    cube.transform.parent = this.transform;
                    cube.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    public void GetDetails()
    {
        GetCubes();
        blockDetails = "";
        halfX = (int)dimensions.x / 2;
        halfZ = (int)dimensions.z / 2;
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    //(new Vector3Int(0, 1, 0), MeshUtils.BlockType.WOOD)
                    if (allCubes[x, y, z] == null) continue;
                    Debug.Log(allCubes[x, y, z].GetComponent<Renderer>().sharedMaterial);
                    if (allCubes[x,y,z].GetComponent<Renderer>().sharedMaterial.ToString().Contains("trunk"))
                        blockDetails += "(new Vector3Int(" + (x - halfX) + "," + y + "," + (z - halfZ) + "), MeshUtils.BlockType.WOOD),\n";
                    else
                        blockDetails += "(new Vector3Int(" + (x - halfX) + "," + y + "," + (z - halfZ) + "), MeshUtils.BlockType.LEAVES),\n";

                }
            }
        }
    }

    public void ReAlignBlocks()
    {
        GetCubes();
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    if(allCubes[x,y,z] != null)
                        allCubes[x, y, z].transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    void GetCubes()
    {
        allCubes = new GameObject[(int)dimensions.x, (int)dimensions.y, (int)dimensions.z];
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    allCubes[x, y, z] = GameObject.Find(x + "|" + y + "|" + z);
                }
            }
        }
    }

    void Draw()
    {
        MeshRenderer[] cubes = this.GetComponentsInChildren<MeshRenderer>();
        if (cubes.Length == 0)
            CreateCubes();

        if (cubes.Length == 0) return;
    }

    void OnValidate()
    {
        Draw();
    }
}
