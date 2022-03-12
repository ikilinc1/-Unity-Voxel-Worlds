using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityStandardAssets.Utility;

[Serializable]
public class WorldData
{
    //private HashSet<Vector3Int> chunkChecker = new HashSet<Vector3Int>();
    //private HashSet<Vector2Int> chunkColumns = new HashSet<Vector2Int>();
    //private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    public int[] chunkCheckerValues;
    public int[] chunkColumnValues;
    public int[] allChunkData;

    public int fpcX;
    public int fpcY;
    public int fpcZ;
    
    public WorldData(){ }

    public WorldData(HashSet<Vector3Int> cChecker, HashSet<Vector2Int> cColumns, Dictionary<Vector3Int, Chunk> chks, Vector3 fpc)
    {
        chunkCheckerValues = new int[cChecker.Count * 3];
        int index = 0;
        foreach (Vector3Int v in cChecker)
        {
            chunkCheckerValues[index] = v.x;
            chunkCheckerValues[index+1] = v.y;
            chunkCheckerValues[index+2] = v.z;
            index = index + 3;
        }

        chunkColumnValues = new int[cColumns.Count * 2];
        index = 0;
        foreach (Vector2Int v in cColumns)
        {
            chunkColumnValues[index] = v.x;
            chunkColumnValues[index+1] = v.y;
            index = index + 2;
        }

        allChunkData = new int[chks.Count * WorldBuilder.chunkDimentions.x * WorldBuilder.chunkDimentions.y * WorldBuilder.chunkDimentions.z];
        index = 0;
        foreach (KeyValuePair<Vector3Int, Chunk> ch in chks)
        {
            foreach (MeshUtils.BlockType bt in ch.Value.chunkData)
            {
                allChunkData[index] = (int) bt;
                index++;
            }
        }

        fpcX = (int) fpc.x;
        fpcY = (int) fpc.y;
        fpcZ = (int) fpc.z;
    }
}

public static class FileSaver
{
    private static WorldData wd;

    static string BuildFileName()
    {
        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath + "/savedata/World_" +
               WorldBuilder.chunkDimentions.x + "-" +
               WorldBuilder.chunkDimentions.y + "-" +
               WorldBuilder.chunkDimentions.z + "_" +
               WorldBuilder.worldDimensions.x + "-" +
               WorldBuilder.worldDimensions.y + "-" +
               WorldBuilder.worldDimensions.z + ".sav";
    }

    public static void save(WorldBuilder world)
    {
        string fileName = BuildFileName();
        if (!File.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(fileName, FileMode.OpenOrCreate);
        wd = new WorldData(world.chunkChecker, world.chunkColumns, world.chunks, world.fPC.transform.position);
        bf.Serialize(file, wd);
        file.Close();
        Debug.Log("Saving World to File -> " + fileName);
    }
}
