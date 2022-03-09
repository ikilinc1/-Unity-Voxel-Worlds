using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{

    public static Vector3 worldDimensions = new Vector3(10, 10, 10);
    public static Vector3 chunkDimentions = new Vector3(10,10,10);
    public GameObject chunkPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
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
                    yield return null;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
