using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class Chunk : MonoBehaviour
{
    public Material atlas;
    
    public int width = 2;
    public int height = 2;
    public int depth = 2;

    public Vector3 location;

    public Block[,,] blocks;
    // Flat formula --> [x + WIDTH * (y + DEPTH * z)] = Original [x, y, z]
    // Flat to normal --> x=i%WIDTH, y=(i/WIDTH)%HEIGHT, z=i/(WIDTH*HEIGHT)
    public MeshUtils.BlockType[] chunkData;
    public MeshUtils.BlockType[] healthData;
    public MeshRenderer meshRenderer;

    private CalculateBlockTypes calculateBlockTypes;
    private JobHandle jobHandle;
    public NativeArray<Unity.Mathematics.Random> randomArray { get; private set; }

    struct CalculateBlockTypes: IJobParallelFor
    {
        public NativeArray<MeshUtils.BlockType> cData;
        public NativeArray<MeshUtils.BlockType> hData;
        public int width;
        public int height;
        public Vector3 location;
        public NativeArray<Unity.Mathematics.Random> randoms;

        public void Execute(int i)
        {
            int x = i % width + (int)location.x;
            int y = (i / width) % height + (int)location.y;
            int z = i / (width * height) + (int)location.z;

            var random = randoms[i];

            int surfaceHeight =(int)MeshUtils.fBM(x, z, WorldBuilder.surfaceSettings.octaves, WorldBuilder.surfaceSettings.scale,
                WorldBuilder.surfaceSettings.heightScale, WorldBuilder.surfaceSettings.heightOffset);
            
            int stoneHeight =(int)MeshUtils.fBM(x, z, WorldBuilder.stoneSettings.octaves, WorldBuilder.stoneSettings.scale,
                WorldBuilder.stoneSettings.heightScale, WorldBuilder.stoneSettings.heightOffset);
            
            int goldTHeight =(int)MeshUtils.fBM(x, z, WorldBuilder.goldTSettings.octaves, WorldBuilder.goldTSettings.scale,
                WorldBuilder.goldTSettings.heightScale, WorldBuilder.goldTSettings.heightOffset);
            
            int goldBHeight =(int)MeshUtils.fBM(x, z, WorldBuilder.goldBSettings.octaves, WorldBuilder.goldBSettings.scale,
                WorldBuilder.goldBSettings.heightScale, WorldBuilder.goldBSettings.heightOffset);
            
            int digCave =(int)MeshUtils.fBM3D(x, y , z, WorldBuilder.caveSettings.octaves, WorldBuilder.caveSettings.scale,
                WorldBuilder.caveSettings.heightScale, WorldBuilder.caveSettings.heightOffset);

            hData[i] = MeshUtils.BlockType.NOCRACK;
            
            if (y == 0)
            {
                cData[i] = MeshUtils.BlockType.BEDROCK;
                return;
            }
            
            if (digCave < WorldBuilder.caveSettings.probability)
            {
                cData[i] = MeshUtils.BlockType.AIR;
                return;
            }
            
            if (surfaceHeight == y && random.NextFloat(1) <= WorldBuilder.surfaceSettings.probability)
            {
                cData[i] = MeshUtils.BlockType.GRASSSIDE;
            }
            else if (y< goldTHeight && y> goldBHeight && random.NextFloat(1) <= WorldBuilder.goldTSettings.probability)
            {
                cData[i] = MeshUtils.BlockType.GOLD;
            }
            else if (y < stoneHeight && random.NextFloat(1) <= WorldBuilder.stoneSettings.probability)
            {
                cData[i] = MeshUtils.BlockType.STONE;
            }
            else if (y<surfaceHeight)
            {
                cData[i] = MeshUtils.BlockType.DIRT;
            }
            else
            {
                cData[i] = MeshUtils.BlockType.AIR;
            }
        }
    }
    
    void BuildChunk()
    {
        int blockCount = width * depth * height;
        chunkData = new MeshUtils.BlockType[blockCount];
        healthData = new MeshUtils.BlockType[blockCount];
        NativeArray<MeshUtils.BlockType> blockTypes =
            new NativeArray<MeshUtils.BlockType>(chunkData, Allocator.Persistent);
        NativeArray<MeshUtils.BlockType> healthTypes =
            new NativeArray<MeshUtils.BlockType>(healthData, Allocator.Persistent);

        var rArray = new Unity.Mathematics.Random[blockCount];
        var seed = new System.Random();

        for (int i = 0; i < blockCount; i++)
        {
            rArray[i] = new Unity.Mathematics.Random((uint) seed.Next());
        }

        randomArray = new NativeArray<Unity.Mathematics.Random>(rArray, Allocator.Persistent);
        
        calculateBlockTypes = new CalculateBlockTypes()
        {
            cData = blockTypes,
            hData = healthTypes,
            width = width,
            height = height,
            location = location,
            randoms = randomArray
        };

        jobHandle = calculateBlockTypes.Schedule(chunkData.Length, 64);
        jobHandle.Complete();
        calculateBlockTypes.cData.CopyTo(chunkData);
        calculateBlockTypes.hData.CopyTo(healthData);
        blockTypes.Dispose();
        healthTypes.Dispose();
        randomArray.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void CreateChunk(Vector3 dimensions, Vector3 position, bool rebuildBlocks = true)
    {
        location = position;
        width = (int) dimensions.x;
        height = (int) dimensions.y;
        depth = (int) dimensions.z;
        
        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = atlas;

        blocks = new Block[width, height, depth];
        if (rebuildBlocks)
        {
            BuildChunk();
        }
        
        var inputMeshes = new List<Mesh>();
        int vertexStart = 0;
        int triStart = 0;
        int meshCount = width * height * depth;
        int _counter = 0;
        var jobs = new ProcessMeshDataJob();
        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.triStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x, y, z] = new Block(new Vector3(x, y, z) + location, chunkData[x + width * (y + depth * z)], this, healthData[x + width * (y + depth * z)]);
                    if (blocks[x, y, z].mesh != null)
                    {
                        inputMeshes.Add(blocks[x, y, z].mesh);
                        var vCount = blocks[x, y, z].mesh.vertexCount;
                        var iCount = (int) blocks[x, y, z].mesh.GetIndexCount(0);
                        jobs.vertexStart[_counter] = vertexStart;
                        jobs.triStart[_counter] = triStart;
                        vertexStart += vCount;
                        triStart += iCount;
                        _counter++;
                    }
                }
            }
        }

        // start job handling
        jobs.meshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
        var outputMeshData = Mesh.AllocateWritableMeshData(1);
        jobs.outputMesh = outputMeshData[0];
        jobs.outputMesh.SetIndexBufferParams(triStart,IndexFormat.UInt32);
        jobs.outputMesh.SetVertexBufferParams(vertexStart, 
            new VertexAttributeDescriptor(VertexAttribute.Position), 
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream:1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream:2),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord1, stream:3));

        var handle = jobs.Schedule(inputMeshes.Count, 4);
        var newMesh = new Mesh();
        newMesh.name = "Chunk_" + location.x + "_" + location.y + "_" + location.z;
        var sm = new SubMeshDescriptor(0, triStart, MeshTopology.Triangles);
        sm.firstVertex = 0;
        sm.vertexCount = vertexStart;
        
        handle.Complete();

        jobs.outputMesh.subMeshCount = 1;
        jobs.outputMesh.SetSubMesh(0,sm);
        
        Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new []{newMesh});
        jobs.meshData.Dispose();
        jobs.vertexStart.Dispose();
        jobs.triStart.Dispose();
        
        newMesh.RecalculateBounds();
        meshFilter.mesh = newMesh;
        MeshCollider collider = this.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = meshFilter.mesh;
    }
    
    [BurstCompile]
    struct ProcessMeshDataJob: IJobParallelFor
    {
        [ReadOnly] public Mesh.MeshDataArray meshData;
        public Mesh.MeshData outputMesh;
        public NativeArray<int> vertexStart;
        public NativeArray<int> triStart;

        public void Execute(int index)
        {
            var data = meshData[index];
            var vCount = data.vertexCount;
            var vStart = vertexStart[index];

            var verts = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(verts.Reinterpret<Vector3>());

            var normals = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());
            
            var uvs = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            // used vector3 for uvs because vector2 give 'weird' results in this job system
            data.GetUVs(0,uvs.Reinterpret<Vector3>());
            
            var uvs2 = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            // used vector3 for uvs because vector2 give 'weird' results in this job system
            data.GetUVs(1,uvs2.Reinterpret<Vector3>());

            var outputVerts = outputMesh.GetVertexData<Vector3>();
            var outputNormals = outputMesh.GetVertexData<Vector3>(stream:1);
            var outputUVs = outputMesh.GetVertexData<Vector3>(stream:2);
            var outputUVs2 = outputMesh.GetVertexData<Vector3>(stream:3);

            for (int i = 0; i < vCount; i++)
            {
                outputVerts[i + vStart] = verts[i];
                outputNormals[i + vStart] = normals[i];
                outputUVs[i + vStart] = uvs[i];
                outputUVs2[i + vStart] = uvs2[i];
            }

            // disposing native arrays (memory leak otherwise)
            verts.Dispose();
            normals.Dispose();
            uvs.Dispose();
            uvs2.Dispose();

            // Triangles starts from here
            var tStart = triStart[index];
            var tCount = data.GetSubMesh(0).indexCount;
            var outputTris = outputMesh.GetIndexData<int>();
            // some systems use 16 some 32 so check for it (prevent overflowing memory)
            if (data.indexFormat == IndexFormat.UInt16)
            {
                var tris = data.GetIndexData<ushort>();
                for (int i = 0; i < tCount; i++)
                {
                    int idx = tris[i];
                    outputTris[i + tStart] = vStart + idx;
                }
            }
            else
            {
                var tris = data.GetIndexData<int>();
                for (int i = 0; i < tCount; i++)
                {
                    int idx = tris[i];
                    outputTris[i + tStart] = vStart + idx;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
