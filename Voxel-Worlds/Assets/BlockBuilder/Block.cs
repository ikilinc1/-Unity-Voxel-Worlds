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
    
    public BlockSide side = BlockSide.FRONT;
    
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        Quad quad = new Quad();
        meshFilter.mesh = quad.Build(side, new Vector3(1,1,1));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
