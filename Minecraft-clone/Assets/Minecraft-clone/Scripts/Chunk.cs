using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Material material;
    public Block[,,] chunkData;

    private MeshFilter[] meshFilter;

    public GameObject chunk;

    public enum StateChunk
    {
        Draw,
        Done,
        Save
    }

    public StateChunk state;

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

                    if (Noise.Caverna(chunk.transform.position.x + x, chunk.transform.position.y + y, chunk.transform.position.z + z, 0.05f, 3) < 0.4f)
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Air, pos, chunk, this);
                    }
                    else if (chunk.transform.position.y + y <= Noise.GeraAlturaRocha(chunk.transform.position.x + x, chunk.transform.position.z + z))
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Rock, pos, chunk, this);
                    }
                    else if (chunk.transform.position.y + y == Noise.GeraAltura(chunk.transform.position.x + x, chunk.transform.position.z + z))
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Grass, pos, chunk, this);
                    }
                    else if (chunk.transform.position.y + y < Noise.GeraAltura(chunk.transform.position.x + x, chunk.transform.position.z + z))
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Dirt, pos, chunk, this);
                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Air, pos, chunk, this);
                    }

                    state = StateChunk.Draw;
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
        MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
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

        if (chunk.gameObject.GetComponent<MeshFilter>() == null)
        {


            MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));
            mf.mesh = new Mesh();


            mf.mesh.CombineMeshes(combine);
        }

        if (chunk.gameObject.GetComponent<MeshRenderer>() == null)
        {
            MeshRenderer renderer = (MeshRenderer)chunk.gameObject.AddComponent(typeof(MeshRenderer));
            renderer.material = material;
        }

        World.DestroyObjects(chunk);
    }
}
