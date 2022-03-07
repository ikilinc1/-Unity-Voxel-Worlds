using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

	public GameObject block;
	public int width = 1;
	public int height = 1;
	public int depth = 1;
	
	public IEnumerator BuildWorld()
	{
		for(int z = 0; z < depth; z++)
			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{
					Vector3 pos = new Vector3(x,y,z);
					GameObject cube = GameObject.Instantiate(block, pos, Quaternion.identity);
					cube.name = x + "_" + y + "_" + z;
					cube.GetComponent<Renderer>().material = new Material (Shader.Find("Standard"));
				}
				yield return null;
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
