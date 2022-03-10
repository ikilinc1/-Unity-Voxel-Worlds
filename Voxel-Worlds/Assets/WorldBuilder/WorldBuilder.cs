using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

public struct PerlinSettings
{
    public float heightScale;
    public float scale;
    public int octaves;
    public float heightOffset;
    public float probability;

    public PerlinSettings(float hs, float s, int o, float ho, float p)
    {
        heightScale = hs;
        scale = s;
        octaves = o;
        heightOffset = ho;
        probability = p;
    }
}
public class WorldBuilder : MonoBehaviour
{

    public static Vector3 worldDimensions = new Vector3(3, 3, 3);
    public static Vector3 chunkDimentions = new Vector3(10,10,10);
    public GameObject chunkPrefab;
    public GameObject mCamera;
    public GameObject fPC;
    public Slider loadingBar;

    public static PerlinSettings surfaceSettings;
    public PerlinGrapher surface;
    
    public static PerlinSettings stoneSettings;
    public PerlinGrapher stone;
    
    public static PerlinSettings goldTSettings;
    public PerlinGrapher goldT;
    
    public static PerlinSettings goldBSettings;
    public PerlinGrapher goldB;
    
    // Start is called before the first frame update
    void Start()
    {
        loadingBar.maxValue = worldDimensions.x * worldDimensions.y * worldDimensions.z;

        surfaceSettings = new PerlinSettings(surface.heightScale, surface.scale, surface.octaves, surface.heighOffset,
            surface.probability);
        
        stoneSettings = new PerlinSettings(stone.heightScale, stone.scale, stone.octaves, stone.heighOffset,
            stone.probability);
        
        goldTSettings = new PerlinSettings(goldT.heightScale, goldT.scale, goldT.octaves, goldT.heighOffset,
            goldT.probability);
        
        goldBSettings = new PerlinSettings(goldB.heightScale, goldB.scale, goldB.octaves, goldB.heighOffset,
            goldB.probability);
        
        StartCoroutine(BuildWorld());
    }
    
    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int y = 0; y < worldDimensions.y; y++)
            {
                for (int x = 0; x < worldDimensions.x; x++)
                {
                    GameObject chunk = Instantiate(chunkPrefab);
                    Vector3 position = new Vector3(x * chunkDimentions.x,y * chunkDimentions.y,z * chunkDimentions.z);
                    chunk.GetComponent<Chunk>().CreateChunk(chunkDimentions,position);
                    loadingBar.value++;
                    yield return null;
                }
            }
        }
        
        //switch cameras
        float xPos = (worldDimensions.x * chunkDimentions.x) / 2.0f;
        float zPos = (worldDimensions.z * chunkDimentions.z) / 2.0f;
        Chunk c = chunkPrefab.GetComponent<Chunk>();
        float yPos = MeshUtils.fBM(xPos, zPos, surfaceSettings.octaves, surfaceSettings.scale, surfaceSettings.heightScale, surfaceSettings.heightOffset)+10.0f;
        fPC.transform.position = new Vector3(xPos, yPos, zPos);
        loadingBar.gameObject.SetActive(false);
        
        mCamera.SetActive(false);
        fPC.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
