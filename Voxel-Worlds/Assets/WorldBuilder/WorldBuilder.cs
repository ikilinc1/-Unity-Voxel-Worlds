using System;
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

    public static Vector3Int worldDimensions = new Vector3Int(5, 5, 5);
    public static Vector3Int extraWorldDimensions = new Vector3Int(5, 5, 5);
    public static Vector3Int chunkDimentions = new Vector3Int(10,10,10);
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
    
    public static PerlinSettings caveSettings;
    public Perlin3DGrapher caves;

    private HashSet<Vector3Int> chunkChecker = new HashSet<Vector3Int>();
    private HashSet<Vector2Int> chunkColumns = new HashSet<Vector2Int>();
    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    private Vector3Int lastBuildPosition;
    public int drawRadius = 3;

    private Queue<IEnumerator> buildQueue = new Queue<IEnumerator>();

    private MeshUtils.BlockType buildType = MeshUtils.BlockType.DIRT;

    IEnumerator BuildCoordinator()
    {
        while (true)
        {
            while (buildQueue.Count > 0)
            {
                yield return StartCoroutine(buildQueue.Dequeue());
            }

            yield return null;
        }
    }

    IEnumerator BuildRecursiveWorld(int x, int z, int radius)
    {
        int nextRad = radius - 1;
        if (radius <= 0)
        {
            yield break;
        }
        BuildChunkColumn(x, z + chunkDimentions.z);
        buildQueue.Enqueue(BuildRecursiveWorld(x, z+chunkDimentions.z, nextRad));
        yield return null;
        
        BuildChunkColumn(x, z - chunkDimentions.z);
        buildQueue.Enqueue(BuildRecursiveWorld(x, z - chunkDimentions.z, nextRad));
        yield return null;
        
        BuildChunkColumn(x + chunkDimentions.x, z);
        buildQueue.Enqueue(BuildRecursiveWorld(x + chunkDimentions.x, z, nextRad));
        yield return null;
        
        BuildChunkColumn(x - chunkDimentions.x, z);
        buildQueue.Enqueue(BuildRecursiveWorld(x - chunkDimentions.x, z, nextRad));
        yield return null;
    }

    private WaitForSeconds wfs = new WaitForSeconds(0.5f);
    IEnumerator UpdateWorld()
    {
        while (true)
        {
            if ((lastBuildPosition - fPC.transform.position).magnitude > chunkDimentions.x)
            {
                var fPCposition = fPC.transform.position;
                lastBuildPosition = Vector3Int.CeilToInt(fPCposition);
                int posX = (int) (fPCposition.x / chunkDimentions.x ) * chunkDimentions.x;
                int posZ = (int) (fPCposition.z / chunkDimentions.z ) * chunkDimentions.z;
                buildQueue.Enqueue(BuildRecursiveWorld(posX, posZ, drawRadius));
                buildQueue.Enqueue(HideColums(posX, posZ));
            }

            yield return wfs;
        }
    }

    IEnumerator HideColums(int x, int z)
    {
        Vector2Int fPCPos = new Vector2Int(x, z);
        foreach (Vector2Int cc in chunkColumns)
        {
            if ((cc - fPCPos).magnitude >= drawRadius * chunkDimentions.x)
            {
                HideChunkColumn(cc.x, cc.y);
            }
        }

        yield return null;
    }
    
    public void HideChunkColumn(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int pos = new Vector3Int(x, y * chunkDimentions.y, z);
            if (chunkChecker.Contains(pos))
            {
                chunks[pos].meshRenderer.enabled = false;
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        loadingBar.maxValue = worldDimensions.x * worldDimensions.z;

        surfaceSettings = new PerlinSettings(surface.heightScale, surface.scale, surface.octaves, surface.heighOffset,
            surface.probability);
        
        stoneSettings = new PerlinSettings(stone.heightScale, stone.scale, stone.octaves, stone.heighOffset,
            stone.probability);
        
        goldTSettings = new PerlinSettings(goldT.heightScale, goldT.scale, goldT.octaves, goldT.heighOffset,
            goldT.probability);
        
        goldBSettings = new PerlinSettings(goldB.heightScale, goldB.scale, goldB.octaves, goldB.heighOffset,
            goldB.probability);
        
        caveSettings = new PerlinSettings(caves.heightScale, caves.scale, caves.octaves, caves.heighOffset,
            caves.drawCutOff);
        
        StartCoroutine(BuildWorld());
    }
    
    public void SetBuildType(int type)
    {
        buildType = (MeshUtils.BlockType) type;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 5))//block click distance
            {
                Vector3 hitBlock = Vector3.zero;
                if (Input.GetMouseButtonDown(0))
                {
                    hitBlock = hit.point - hit.normal / 2.0f;
                }
                else
                {
                    hitBlock = hit.point + hit.normal / 2.0f;
                }

                Chunk thisChunk = hit.collider.gameObject.GetComponent<Chunk>();

                int bx = (int) (Mathf.Round(hitBlock.x) - thisChunk.location.x);
                int by = (int) (Mathf.Round(hitBlock.y) - thisChunk.location.y);
                int bz = (int) (Mathf.Round(hitBlock.z) - thisChunk.location.z);

                //calculatin neighbour chunks
                Vector3Int neighbour;
                if (bx == chunkDimentions.x)
                {
                    neighbour = new Vector3Int((int) thisChunk.location.x + chunkDimentions.x,
                        (int) thisChunk.location.y,
                        (int) thisChunk.location.z);
                    thisChunk = chunks[neighbour];
                    bx = 0;
                }else if (bx == -1)
                {
                    neighbour = new Vector3Int((int) thisChunk.location.x - chunkDimentions.x,
                        (int) thisChunk.location.y,
                        (int) thisChunk.location.z);
                    thisChunk = chunks[neighbour];
                    bx = chunkDimentions.x - 1;
                }
                
                if (by == chunkDimentions.y)
                {
                    neighbour = new Vector3Int((int) thisChunk.location.x,
                        (int) thisChunk.location.y + chunkDimentions.y,
                        (int) thisChunk.location.z);
                    thisChunk = chunks[neighbour];
                    by = 0;
                }else if (by == -1)
                {
                    neighbour = new Vector3Int((int) thisChunk.location.x ,
                        (int) thisChunk.location.y - chunkDimentions.y,
                        (int) thisChunk.location.z);
                    thisChunk = chunks[neighbour];
                    by = chunkDimentions.y - 1;
                }
                
                if (bz == chunkDimentions.z)
                {
                    neighbour = new Vector3Int((int) thisChunk.location.x,
                        (int) thisChunk.location.y,
                        (int) thisChunk.location.z + chunkDimentions.z);
                    thisChunk = chunks[neighbour];
                    bz = 0;
                }else if (bz == -1)
                {
                    neighbour = new Vector3Int((int) thisChunk.location.x,
                        (int) thisChunk.location.y,
                        (int) thisChunk.location.z - chunkDimentions.z);
                    thisChunk = chunks[neighbour];
                    bz = chunkDimentions.z - 1;
                }
                //---------------------------
                int i = bx + chunkDimentions.x * (by + chunkDimentions.z * bz); // flatten
                
                if (Input.GetMouseButtonDown(0))
                {
                    if (MeshUtils.blockTypeHealth[(int) thisChunk.chunkData[i]] != -1)
                    {
                        if (thisChunk.healthData[i] == MeshUtils.BlockType.NOCRACK)
                        {
                            StartCoroutine(HealBlock(thisChunk, i));
                        }
                        thisChunk.healthData[i]++;
                        if (thisChunk.healthData[i] == MeshUtils.BlockType.NOCRACK +
                            MeshUtils.blockTypeHealth[(int) thisChunk.chunkData[i]])
                        {
                            thisChunk.chunkData[i] = MeshUtils.BlockType.AIR;
                        }
                    }

                }
                else
                {
                    thisChunk.chunkData[i] = buildType;
                    thisChunk.healthData[i] = MeshUtils.BlockType.NOCRACK;
                }
                
                RedrawChunk(thisChunk);
            }
        }
    }

    void RedrawChunk(Chunk c)
    {
        DestroyImmediate(c.GetComponent<MeshFilter>());
        DestroyImmediate(c.GetComponent<MeshRenderer>());
        DestroyImmediate(c.GetComponent<Collider>());
                
        c.CreateChunk(chunkDimentions,c.location,false);
    }
    
    private WaitForSeconds threeSeconds = new WaitForSeconds(3);
    public IEnumerator HealBlock(Chunk c, int blockIndex)
    {
        yield return threeSeconds;
        if (c.chunkData[blockIndex] != MeshUtils.BlockType.AIR)
        {
            c.healthData[blockIndex] = MeshUtils.BlockType.NOCRACK;
            RedrawChunk(c);
        }
    }
    

    void BuildChunkColumn(int x, int z, bool meshEnabled = true)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int position = new Vector3Int(x, y * chunkDimentions.y, z);
            if (!chunkChecker.Contains(position))
            {
                GameObject chunk = Instantiate(chunkPrefab);
                chunk.name = "Chunk_" + position.x + "_" + position.y + "_" + position.z;
                Chunk c = chunk.GetComponent<Chunk>();
                c.CreateChunk(chunkDimentions,position);
                chunkChecker.Add(position);
                chunks.Add(position, c);   
            }
            chunks[position].meshRenderer.enabled = meshEnabled;
            
        }
        chunkColumns.Add(new Vector2Int(x, z));
    }

    IEnumerator BuildExtraWorld()
    {
        int zStart = worldDimensions.z ;
        int zEnd = worldDimensions.z + extraWorldDimensions.z;
        
        int xStart = worldDimensions.x;
        int xEnd = worldDimensions.x + extraWorldDimensions.x;
        
        for (int z = zStart; z < zEnd; z++)
        {
            for (int x = 0; x < xEnd; x++)
            {
                BuildChunkColumn(x * chunkDimentions.x, z * chunkDimentions.z, false);
                yield return null;
            }
        }
        
        for (int z = 0; z < zEnd; z++)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                BuildChunkColumn(x * chunkDimentions.x, z * chunkDimentions.z, false);
                yield return null;
            }
        }
    }

    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
             for (int x = 0; x < worldDimensions.x; x++)
             {
                 BuildChunkColumn(x * chunkDimentions.x,z * chunkDimentions.z);
                 loadingBar.value++;
                 yield return null;
             }
        }
        
        //switch cameras
        int xPos = (worldDimensions.x * chunkDimentions.x) / 2;
        int zPos = (worldDimensions.z * chunkDimentions.z) / 2;
        int yPos = (int)MeshUtils.fBM(xPos, zPos, surfaceSettings.octaves, surfaceSettings.scale, surfaceSettings.heightScale, surfaceSettings.heightOffset)+10;
        fPC.transform.position = new Vector3Int(xPos, yPos, zPos);
        loadingBar.gameObject.SetActive(false);
        
        mCamera.SetActive(false);
        fPC.SetActive(true);
        lastBuildPosition = Vector3Int.CeilToInt(fPC.transform.position);
        StartCoroutine(BuildCoordinator());
        //StartCoroutine(UpdateWorld()); // temp
        StartCoroutine(BuildExtraWorld());
    }
    
}
