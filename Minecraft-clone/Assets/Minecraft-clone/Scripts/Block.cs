using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Block
{
    public bool solid;
    public Chunk dono;

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
        dono = c;
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

    public bool TemVizinhoSolido(int x, int y, int z)
    {
        Block[,,] chunks;
        chunks = dono.chunkData;

        try
        {
            return chunks[x, y, z].solid;
        }
        catch (System.IndexOutOfRangeException ex)
        {
            return false;
        }
    }

    [SerializeField] public Mesh mesh;
    [SerializeField] public MeshFilter meshFilter;
    [SerializeField] private MeshCollider chunkCol;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangulos = new List<int>();

    [SerializeField] private int contFace;

    //Textura

    public List<Vector2> UVText = new List<Vector2>();
    private float textLargura = 0.0625f;
    private int faceContagem;

    private Vector2 gramaTopo = new Vector2(2, 6);
    private Vector2 gramaLado = new Vector2(3, 15);
    private Vector2 gramaBaixo = new Vector2(2, 15);

    void TexturaAjuste(Vector2 info)
    {
        Vector2 posTextura;
        posTextura = info;

        UVText.Add(new Vector2(textLargura * posTextura.x + textLargura, textLargura * posTextura.y));
        UVText.Add(new Vector2(textLargura * posTextura.x + textLargura, textLargura * posTextura.y + textLargura));
        UVText.Add(new Vector2(textLargura * posTextura.x, textLargura * posTextura.y + textLargura));
        UVText.Add(new Vector2(textLargura * posTextura.x, textLargura * posTextura.y));
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
        vertices.Add(new Vector3(x, y, z + 1));
        vertices.Add(new Vector3(x + 1, y, z + 1));
        vertices.Add(new Vector3(x + 1, y, z));
        vertices.Add(new Vector3(x, y, z));

        TexturaAjuste(gramaTopo);

        CalcTris();
    }

    void NorteConstr(int x, int y, int z)
    {
        vertices.Add(new Vector3(x + 1, y - 1, z + 1));
        vertices.Add(new Vector3(x + 1, y, z + 1));
        vertices.Add(new Vector3(x, y, z + 1));
        vertices.Add(new Vector3(x, y - 1, z + 1));

        TexturaAjuste(gramaLado);

        CalcTris();
    }

    void LesteConstr(int x, int y, int z)
    {
        vertices.Add(new Vector3(x + 1, y - 1, z));
        vertices.Add(new Vector3(x + 1, y, z));
        vertices.Add(new Vector3(x + 1, y, z + 1));
        vertices.Add(new Vector3(x + 1, y - 1, z + 1));

        TexturaAjuste(gramaLado);

        CalcTris();
    }

    void SulConstr(int x, int y, int z)
    {
        vertices.Add(new Vector3(x, y - 1, z));
        vertices.Add(new Vector3(x, y, z));
        vertices.Add(new Vector3(x + 1, y, z));
        vertices.Add(new Vector3(x + 1, y - 1, z));

        TexturaAjuste(gramaLado);

        CalcTris();
    }

    void OesteConstr(int x, int y, int z)
    {
        vertices.Add(new Vector3(x, y - 1, z + 1));
        vertices.Add(new Vector3(x, y, z + 1));
        vertices.Add(new Vector3(x, y, z));
        vertices.Add(new Vector3(x, y - 1, z));

        TexturaAjuste(gramaLado);

        CalcTris();
    }

    void BaixoConstr(int x, int y, int z)
    {
        vertices.Add(new Vector3(x, y - 1, z));
        vertices.Add(new Vector3(x + 1, y - 1, z));
        vertices.Add(new Vector3(x + 1, y - 1, z + 1));
        vertices.Add(new Vector3(x, y - 1, z + 1));

        TexturaAjuste(gramaBaixo);

        CalcTris();
    }

    void CalcTris()
    {
        triangulos.Add(contFace * 4 + 0);
        triangulos.Add(contFace * 4 + 1);
        triangulos.Add(contFace * 4 + 2);
        triangulos.Add(contFace * 4 + 0);
        triangulos.Add(contFace * 4 + 2);
        triangulos.Add(contFace * 4 + 3);

        contFace++;
    }

    void MeshUpdate()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.uv = UVText.ToArray();
        mesh.triangles = triangulos.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        vertices.Clear();
        UVText.Clear();
        triangulos.Clear();

        contFace = 0;
    }
}
