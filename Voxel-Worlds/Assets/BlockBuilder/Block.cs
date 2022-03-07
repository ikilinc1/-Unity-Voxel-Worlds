using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [System.Serializable]
    public enum BlockSide
    {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    };

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        Quad[] quads = new Quad[6];
        quads[0] = new Quad(BlockSide.BOTTOM, new Vector3(0, 0, 0));
        quads[1] = new Quad(BlockSide.TOP, new Vector3(0, 0, 0));
        quads[2] = new Quad(BlockSide.LEFT, new Vector3(0, 0, 0));
        quads[3] = new Quad(BlockSide.RIGHT, new Vector3(0, 0, 0));
        quads[4] = new Quad(BlockSide.FRONT, new Vector3(0, 0, 0));
        quads[5] = new Quad(BlockSide.BACK, new Vector3(0, 0, 0));

        Mesh[] sideMeshes = new Mesh[6];
        sideMeshes[0] = quads[0].mesh;
        sideMeshes[1] = quads[1].mesh;
        sideMeshes[2] = quads[2].mesh;
        sideMeshes[3] = quads[3].mesh;
        sideMeshes[4] = quads[4].mesh;
        sideMeshes[5] = quads[5].mesh;

        meshFilter.mesh = MeshUtils.mergeMeshes(sideMeshes);
        meshFilter.mesh.name = "Cube_0_0_0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
