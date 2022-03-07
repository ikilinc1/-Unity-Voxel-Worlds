using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

	public GameObject block;
	public int width = 2;
	public int height = 2;
	public int depth = 2;
	
	public IEnumerator BuildWorld()
	{
		for (int z = 0; z < depth; z++)
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (y >= height - 2 && Random.Range(0,100) < 50)
					{
						continue;
					}
					Vector3 position = new Vector3(x, y, z);
					GameObject cube = GameObject.Instantiate(block, position, Quaternion.identity);
					cube.name = x + "_" + y + "_" + z;
					cube.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
				}
				// yield after 1 row
				yield return null;
			}
		}
	}
	// Use this for initialization
	void Start () {
		StartCoroutine(BuildWorld());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
