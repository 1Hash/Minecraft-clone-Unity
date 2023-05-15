using Realtime.Messaging.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    public Material textureMat;
    public static int columnAlt = 16;
    public static int chunkSize = 16;
    public static int WorldSize = 1;

    //public static Dictionary<string, Chunk> chunks;
    public static ConcurrentDictionary<string, Chunk> chunks;

    public static int radius;
    public GameObject player;

    public Vector3 lastPosConst, movingPlayer;
    public CoroutineQueue queue;
    public uint maxCoro = 500;

    public static List<string> removeCena = new List<string>();

    private static World instance;

    private void Awake()
    {
        radius = 4;

        if (instance == null)
        {
            instance = this;
        }

        //StartCoroutine(UnloadSceneAdd());
    }

    IEnumerator UnloadSceneAdd()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.UnloadSceneAsync(0);
    }

    public static string ChunkName(Vector3 v)
    {
        return (int)v.x + "-" + (int)v.y + "-" + (int)v.z;
    }

    /*IEnumerator ChunkColumnBuild()
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
    }*/

    /*IEnumerator WorldBuild()
    {
        construindo = true;
        int posX = (int)Mathf.Floor(player.transform.position.x / chunkSize);
        int posZ = (int)Mathf.Floor(player.transform.position.z / chunkSize);

        for (int z = -raio; z <= raio; z++)
            for (int x = -raio; x <= raio; x++)
                for (int y = 0; y < columnAlt; y++)
                {
                    Vector3 chunkPos = new Vector3((x + posX) * chunkSize, y * chunkSize, (z + posZ) * chunkSize);

                    Chunk c;
                    string chunkString = ChunkName(chunkPos);

                    if (chunks.TryGetValue(chunkString, out c))
                    {
                        c.estado = Chunk.ChunkEstado.Guarda;
                        break;
                    }
                    else
                    {
                        c = new Chunk(chunkPos, textureMat);
                        c.chunk.transform.parent = this.transform;
                        chunks.Add(c.chunk.name, c);
                    }

                }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.estado == Chunk.ChunkEstado.Desenho)
            {
                c.Value.DrawChunk();
                c.Value.estado = Chunk.ChunkEstado.Guarda;
            }

            c.Value.estado = Chunk.ChunkEstado.Feito;

            yield return null;
        }

        if (primeiraConst)
        {
            primeiraConst = false;
        }

        construindo = false;
    }*/

    void WorldBuild(int x, int y, int z)
    {
        Vector3 chunkPos = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);

        Chunk c;
        string chunkString = ChunkName(chunkPos);

        if (!chunks.TryGetValue(chunkString, out c))
        {
            c = new Chunk(chunkPos, textureMat);
            c.chunk.transform.parent = this.transform;
            chunks.TryAdd(c.chunk.name, c);
        }
    }

    IEnumerator WorldRepetableBuild(int x, int y, int z, int raio)
    {
        raio--;

        if (raio <= 0)
        {
            yield break;
        }

        DirRepetableWorldPositive(true, x, y, z, 0, 0, 1, raio);
        DirRepetableWorldPositive(false, x, y, z, 0, 0, 1, raio);
        DirRepetableWorldPositive(true, x, y, z, 1, 0, 0, raio);
        DirRepetableWorldPositive(false, x, y, z, 1, 0, 0, raio);
        //DirMundoRepetidoPositivoY(true, x, y, z, 0, 1, 0, columnAlt);
        //DirMundoRepetidoPositivoY(false, x, y, z, 0, 1, 0, columnAlt);
        yield return null;
    }

    void DirRepetableWorldPositive(bool positivo, int x, int y, int z, int xi, int yi, int zi, int raio)
    {
        if (positivo)
        {
            WorldBuild(x + xi, y + yi, z + zi);

            for (int i = columnAlt; i >= 0; i--)
            {
                WorldBuild(x - xi, i, z - zi);
            }

            queue.Run(WorldRepetableBuild(x + xi, y + yi, z + zi, raio));
        }
        else
        {
            WorldBuild(x - xi, y - yi, z - zi);

            for (int i = columnAlt; i >= 0; i--)
            {
                WorldBuild(x - xi, i, z - zi);
            }

            queue.Run(WorldRepetableBuild(x - xi, y - y, z - zi, raio));
        }
    }

    IEnumerator DrawCh()
    {
        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.state == Chunk.StateChunk.Draw)
            {
                c.Value.DrawChunk();
            }

            /*if (c.Value.chunk && Vector3.Distance(player.transform.position, c.Value.chunk.transform.position) > raio * chunkSize)
            {
                removeCena.Add(c.Key);
            }*/

            yield return null;
        }
    }

    IEnumerator Removechunk()
    {
        for (int i = 0; i < removeCena.Count; i++)
        {
            string s = removeCena[i];
            Chunk c;

            if (chunks.TryGetValue(s, out c))
            {
                Destroy(c.chunk);
                chunks.TryRemove(s, out c);
                yield return null;
            }
        }
    }

    public void ConstClosePlayer()
    {
        StopCoroutine("WorldRepetableBuild");
        queue.Run(WorldRepetableBuild((int)(player.transform.position.x / chunkSize),
                                           (int)(player.transform.position.y / chunkSize),
                                           (int)(player.transform.position.z / chunkSize), radius));
    }

    void Start()
    {
        lastPosConst = player.transform.position;

        chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        queue = new CoroutineQueue(maxCoro, StartCoroutine);

        //WorldBuild((int)(player.transform.position.x / chunkSize), (int)(player.transform.position.y / chunkSize), (int)(player.transform.position.z / chunkSize));

        queue.Run(WorldRepetableBuild((int)(player.transform.position.x / chunkSize),
                                           (int)(player.transform.position.y / chunkSize),
                                           (int)(player.transform.position.z / chunkSize), radius));

        queue.Run(DrawCh());

        //firsBuild = false;
    }

    void Update()
    {
        movingPlayer = lastPosConst - player.transform.position;

        if (movingPlayer.magnitude >= chunkSize)
        {
            lastPosConst = player.transform.position;
            ConstClosePlayer();
            queue.Run(DrawCh());
            //queue.Run(Removechunk());
        }
    }

    public static void DestroyObjects(GameObject chunk)
    {
        foreach (Transform item in chunk.transform)
        {
            Destroy(item.gameObject);
        }
    }
}
