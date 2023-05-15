using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    static int maxAltura = 100;
    static float smooth = 0.01f;
    static int cicles = 4;
    static float persists = 0.5f;

    static float MoveB(float x, float z, int cicle, float persist)
    {
        float frequency = 1;
        float amplitude = 1;
        float total = 0;
        float maxVal = 0;

        for (int i = 0; i < cicle; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
            maxVal += amplitude;
            amplitude *= persist;
            frequency *= 2;
        }

        return total / maxVal;
    }

    public static float Caverna(float x, float y, float z, float smooth, int cicles)
    {
        float xy = MoveB(x * smooth, y * smooth, cicles, 0.5f);
        float yz = MoveB(y * smooth, z * smooth, cicles, 0.5f);
        float xz = MoveB(x * smooth, z * smooth, cicles, 0.5f);

        float zy = MoveB(z * smooth, y * smooth, cicles, 0.5f);
        float zx = MoveB(z * smooth, x * smooth, cicles, 0.5f);
        float yx = MoveB(y * smooth, z * smooth, cicles, 0.5f);

        return (xy + yz + xz + zy + zx + yx) / 6;
    }

    public static int GeraAltura(float x, float z)
    {
        float altura = Mapa(0, maxAltura, 0, 1, MoveB(x * smooth, z * smooth, cicles, persists));
        return (int)altura;
    }

    public static int GeraAlturaRocha(float x, float z)
    {
        float altura = Mapa(0, maxAltura - 10, 0, 1, MoveB(x * smooth * 2, z * smooth * 2, cicles, persists));
        return (int)altura;
    }

    static float Mapa(float vMin, float vMax, float oriMin, float oriMax, float value)
    {

        return Mathf.Lerp(vMin, vMax, Mathf.InverseLerp(oriMin, oriMax, value));
    }
}
