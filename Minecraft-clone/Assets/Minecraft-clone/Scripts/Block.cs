using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Block
{
    public bool solid;
    public Chunk owner;

    public enum TypeTexture
    {
        Dirt,
        Grass,
        Rock,
        Air
    }

    public TypeTexture typeTexture;
    public Vector3 position;
    public GameObject obj;
    public Material cubeMat;

    public Block(TypeTexture t, Vector3 pos, GameObject p, Chunk c)
    {
        typeTexture = t;
        owner = c;
        position = pos;
        obj = p;

        if (typeTexture == TypeTexture.Air)
        {
            solid = false;
        }
        else
        {
            solid = true;
        }
    }

    int ConvertPos(int a)
    {
        if(a == -1)
        {
            a = World.chunkSize - 1;
        }
        else if(a <= World.chunkSize)
        {
            a = 0;
        }

        return a;
    }

    public bool TemVizinhoSolido(int x, int y, int z)
    {
        Block[,,] chunks;

        if(x < 0 || x >= World.chunkSize ||
            y < 0 || y >= World.chunkSize ||
            z < 0 || z >= World.chunkSize)
        {
            Vector3 posVizinho = this.obj.transform.position + new Vector3(
                (x - (int)position.x) * World.chunkSize,
                (y - (int)position.y) * World.chunkSize,
                (z - (int)position.z) * World.chunkSize);

            string nomeK = World.ChunkName(posVizinho);

            x = ConvertPos(x);
            y = ConvertPos(y);
            z = ConvertPos(z);

            Chunk ch;
            if(World.chunks.TryGetValue(nomeK, out ch))
            {
                chunks = ch.chunkData;
            }
            else
            {
                return false;
            }

        }
        else
        {
            chunks = owner.chunkData;
        }


        try
        {
            return chunks[x, y, z].solid;
        }
        catch (System.IndexOutOfRangeException)
        {
            return false;
        }     
    }

    [SerializeField] public Mesh mesh;
    [SerializeField] public MeshFilter meshFilter;
    [SerializeField] private MeshCollider chunkCol;

    private List<Vector3> vertex = new List<Vector3>();
    private List<int> triangles = new List<int>();

    [SerializeField] private int contFace;

    //Textura

    public List<Vector2> UVText = new List<Vector2>();
    private float textWidth = 0.0625f;

    private Vector2 grassTop = new Vector2(2, 6);
    private Vector2 grassSide = new Vector2(3, 15);
    private Vector2 grassLow = new Vector2(2, 15);

    private Vector2 dirtTotal = new Vector2(2, 15);
    private Vector2 stoneTotal = new Vector2(1, 15);

    void TexturaAjuste(Vector2 info)
    {
        Vector2 posTexture;
        posTexture = info;

        UVText.Add(new Vector2(textWidth * posTexture.x + textWidth, textWidth * posTexture.y));
        UVText.Add(new Vector2(textWidth * posTexture.x + textWidth, textWidth * posTexture.y + textWidth));
        UVText.Add(new Vector2(textWidth * posTexture.x, textWidth * posTexture.y + textWidth));
        UVText.Add(new Vector2(textWidth * posTexture.x, textWidth * posTexture.y));
    }


    void Start()
    {

    }

    public void CreateCube()
    {
        if (typeTexture == TypeTexture.Air) return;

        if (!TemVizinhoSolido((int)position.x, (int)position.y + 1, (int)position.z))
        {
            TopoConstr(0, 0, 0);
        }
        if (!TemVizinhoSolido((int)position.x, (int)position.y, (int)position.z + 1))
        {
            NorteConstr(0, 0, 0);
        }
        if (!TemVizinhoSolido((int)position.x + 1, (int)position.y, (int)position.z))
        {
            LesteConstr(0, 0, 0);
        }
        if (!TemVizinhoSolido((int)position.x, (int)position.y, (int)position.z - 1))
        {
            SulConstr(0, 0, 0);
        }
        if (!TemVizinhoSolido((int)position.x - 1, (int)position.y, (int)position.z))
        {
            OesteConstr(0, 0, 0);
        }
        if (!TemVizinhoSolido((int)position.x, (int)position.y - 1, (int)position.z))
        {
            BaixoConstr(0, 0, 0);
        }

        /*if (!gameObject.GetComponent<MeshFilter>())
        {
            gameObject.AddComponent(typeof(MeshFilter));
        }

        if (!gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.AddComponent(typeof(MeshRenderer));
        }
        
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        gameObject.transform.position = position;
        gameObject.GetComponent<MeshRenderer>().material = cubeMat;*/

        GameObject cube = new GameObject("cube");
        cube.AddComponent(typeof(MeshFilter));
        cube.AddComponent(typeof(MeshRenderer));
        mesh = cube.GetComponent<MeshFilter>().mesh;
        cube.transform.position = position;
        cube.transform.parent = obj.transform;
        cube.GetComponent<MeshRenderer>().material = cubeMat;


        MeshUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TopoConstr(int x, int y, int z)
    {
        vertex.Add(new Vector3(x, y, z + 1));
        vertex.Add(new Vector3(x + 1, y, z + 1));
        vertex.Add(new Vector3(x + 1, y, z));
        vertex.Add(new Vector3(x, y, z));

        if (typeTexture == TypeTexture.Dirt)
        {
            TexturaAjuste(dirtTotal);
        }
        else if (typeTexture == TypeTexture.Grass)
        {
            TexturaAjuste(grassTop);
        }
        else if (typeTexture == TypeTexture.Rock)
        {
            TexturaAjuste(stoneTotal);
        }

        CalcTris();
    }

    void NorteConstr(int x, int y, int z)
    {
        vertex.Add(new Vector3(x + 1, y - 1, z + 1));
        vertex.Add(new Vector3(x + 1, y, z + 1));
        vertex.Add(new Vector3(x, y, z + 1));
        vertex.Add(new Vector3(x, y - 1, z + 1));

        if (typeTexture == TypeTexture.Dirt)
        {
            TexturaAjuste(dirtTotal);
        }
        else if (typeTexture == TypeTexture.Grass)
        {
            TexturaAjuste(grassSide);
        }
        else if (typeTexture == TypeTexture.Rock)
        {
            TexturaAjuste(stoneTotal);
        }

        CalcTris();
    }

    void LesteConstr(int x, int y, int z)
    {
        vertex.Add(new Vector3(x + 1, y - 1, z));
        vertex.Add(new Vector3(x + 1, y, z));
        vertex.Add(new Vector3(x + 1, y, z + 1));
        vertex.Add(new Vector3(x + 1, y - 1, z + 1));

        if (typeTexture == TypeTexture.Dirt)
        {
            TexturaAjuste(dirtTotal);
        }
        else if (typeTexture == TypeTexture.Grass)
        {
            TexturaAjuste(grassSide);
        }
        else if (typeTexture == TypeTexture.Rock)
        {
            TexturaAjuste(stoneTotal);
        }

        CalcTris();
    }

    void SulConstr(int x, int y, int z)
    {
        vertex.Add(new Vector3(x, y - 1, z));
        vertex.Add(new Vector3(x, y, z));
        vertex.Add(new Vector3(x + 1, y, z));
        vertex.Add(new Vector3(x + 1, y - 1, z));

        if (typeTexture == TypeTexture.Dirt)
        {
            TexturaAjuste(dirtTotal);
        }
        else if (typeTexture == TypeTexture.Grass)
        {
            TexturaAjuste(grassSide);
        }
        else if (typeTexture == TypeTexture.Rock)
        {
            TexturaAjuste(stoneTotal);
        }

        CalcTris();
    }

    void OesteConstr(int x, int y, int z)
    {
        vertex.Add(new Vector3(x, y - 1, z + 1));
        vertex.Add(new Vector3(x, y, z + 1));
        vertex.Add(new Vector3(x, y, z));
        vertex.Add(new Vector3(x, y - 1, z));

        if (typeTexture == TypeTexture.Dirt)
        {
            TexturaAjuste(dirtTotal);
        }
        else if (typeTexture == TypeTexture.Grass)
        {
            TexturaAjuste(grassSide);
        }
        else if (typeTexture == TypeTexture.Rock)
        {
            TexturaAjuste(stoneTotal);
        }

        CalcTris();
    }

    void BaixoConstr(int x, int y, int z)
    {
        vertex.Add(new Vector3(x, y - 1, z));
        vertex.Add(new Vector3(x + 1, y - 1, z));
        vertex.Add(new Vector3(x + 1, y - 1, z + 1));
        vertex.Add(new Vector3(x, y - 1, z + 1));

        if (typeTexture == TypeTexture.Dirt)
        {
            TexturaAjuste(dirtTotal);
        }
        else if (typeTexture == TypeTexture.Grass)
        {
            TexturaAjuste(grassLow);
        }
        else if (typeTexture == TypeTexture.Rock)
        {
            TexturaAjuste(stoneTotal);
        }

        CalcTris();
    }

    void CalcTris()
    {
        triangles.Add(contFace * 4 + 0);
        triangles.Add(contFace * 4 + 1);
        triangles.Add(contFace * 4 + 2);
        triangles.Add(contFace * 4 + 0);
        triangles.Add(contFace * 4 + 2);
        triangles.Add(contFace * 4 + 3);

        contFace++;
    }

    void MeshUpdate()
    {
        mesh.Clear();
        mesh.vertices = vertex.ToArray();
        mesh.uv = UVText.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        vertex.Clear();
        UVText.Clear();
        triangles.Clear();

        contFace = 0;
    }
}
