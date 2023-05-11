using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Material material;
    public Block[,,] chunkData;

    private MeshFilter[] meshFilter;

    IEnumerator BuildChunk(int tX, int tY, int tZ)
    {
        chunkData = new Block[tX, tY, tZ];

        //create blocks

        for (int z = 0; z < tZ; z++)
        {
            for(int y = 0; y < tY; y++)
            {
                for (int x = 0; x < tX; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    if (Random.Range(0, 500) > 100)
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Grass, pos, this.gameObject, material);
                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(Block.TypeTexture.Air, pos, this.gameObject, material);
                    }

                    
                }
            }
        }

        //draw blocks

        for (int z = 0; z < tZ; z++)
        {
            for (int y = 0; y < tY; y++)
            {
                for (int x = 0; x < tX; x++)
                {
                    chunkData[x, y, z].CreateCube();
                    yield return null;
                }
            }
        }

        CombineAll();
    }

    void Start()
    {
        StartCoroutine(BuildChunk(4,4,4));
    }

    void CombineAll()
    {
        meshFilter = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilter.Length];

        int m = 0;

        while (m < meshFilter.Length)
        {
            combine[m].mesh = meshFilter[m].sharedMesh;
            combine[m].transform = meshFilter[m].transform.localToWorldMatrix;
            m++;
        }

        MeshFilter mf = (MeshFilter)this.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);

        MeshRenderer renderer = (MeshRenderer)this.gameObject.AddComponent(typeof(MeshRenderer));
        renderer.material = this.material;

        foreach (Transform item in this.transform)
        {
            Destroy(item.gameObject);
        }
    }
}
