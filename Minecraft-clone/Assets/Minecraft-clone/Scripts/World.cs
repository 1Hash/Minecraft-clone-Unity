using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material textureMat;
    public static int columnAlt = 4;
    public static int chunkSize = 8;
    public static int WorldSize = 8;

    public static Dictionary<string, Chunk> chunks;

    private static World instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static string ChunkName(Vector3 v)
    {
        return (int)v.x + "-" + (int)v.y + "-" + (int)v.z;
    }

    IEnumerator ChunkColumnBuild()
    {
        for (int i = 0; i < columnAlt; i++)
        {
            Vector3 chunkPos = new Vector3(this.transform.position.x, i * chunkSize, this.transform.position.z);
            Chunk c = new Chunk(chunkPos, textureMat);
            c.chunk.transform.parent = this.transform;
            chunks.Add(c.chunk.name, c);
        }

        foreach (KeyValuePair<string,Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }
    }

    IEnumerator WorldBuild()
    {
        for (int x = 0; x < WorldSize; x++)
            for (int y = 0; y < columnAlt; y++)
                for (int z = 0; z < WorldSize; z++)
                {
                    Vector3 chunkPos = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    Chunk c = new Chunk(chunkPos, textureMat);
                    c.chunk.transform.parent = this.transform;
                    chunks.Add(c.chunk.name, c);
                }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }
    }

    void Start()
    {
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(WorldBuild());
    }

    void Update()
    {
        
    }

    public static void DestroyObjects(GameObject chunk)
    {
        foreach (Transform item in chunk.transform)
        {
            Destroy(item.gameObject);
        }
    }
}
