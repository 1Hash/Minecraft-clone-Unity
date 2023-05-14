using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Material material;
    public Block[,,] chunkData;

    private MeshFilter[] meshFilter;

    public GameObject chunk;

    void BuildChunk()
    {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        //create blocks

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    if (Random.Range(0, 500) > 100)
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Grass, pos, chunk, this);
                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Air, pos, chunk, this);
                    }


                }
            }
        }

    }

    public void DrawChunk()
    {
        //draw blocks

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunkData[x, y, z].CreateCube();
                }
            }
        }

        CombineAll();
    }

    public Chunk(Vector3 position, Material m)
    {
        chunk = new GameObject(World.ChunkName(position));
        chunk.transform.position = position;
        material = m;
        BuildChunk();
    }

    void Start()
    {
        
    }

    void CombineAll()
    {
        meshFilter = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilter.Length];

        int m = 0;

        while (m < meshFilter.Length)
        {
            combine[m].mesh = meshFilter[m].sharedMesh;
            combine[m].transform = meshFilter[m].transform.localToWorldMatrix;
            m++;
        }

        MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);

        MeshRenderer renderer = (MeshRenderer)chunk.gameObject.AddComponent(typeof(MeshRenderer));
        renderer.material = this.material;

        World.DestroyObjects(chunk);
    }
}
