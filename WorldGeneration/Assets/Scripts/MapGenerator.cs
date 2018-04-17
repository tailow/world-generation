using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Variables

    int offsetX;
    int offsetY;

    int pointX;
    int pointY;

    public Region[] regions;

    public float fallOffStrength;
    public float fallOffStart;
    public float depth;
    public float scale;
    public float lacunarity;

    [Range(0, 1)]
    public float persistance;

    public int octaves;
    public int seed;

    [Range(0, 6)]
    public int levelOfDetail;
    const int size = 97;

    public bool autoUpdate;

    public AnimationCurve meshHeightCurve;

    #endregion

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        System.Random rng = new System.Random(seed);

        offsetX = rng.Next(-50000, 50000);
        offsetY = rng.Next(-50000, 50000);

        GenerateTerrain(GetComponent<MeshFilter>());
    }

    void GenerateTerrain(MeshFilter terrainMesh)
    {
        terrainMesh.sharedMesh = MeshGenerator.GenerateTerrainMesh(GenerateNoiseMap(), meshHeightCurve, levelOfDetail, depth).CreateMesh();

        terrainMesh.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = GenerateColorMap();
    }

    float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                noiseMap[x, y] = GetPointHeight(new Vector2(x, y));
            }
        }

        return noiseMap;
    }

    float GetPointHeight(Vector2 point)
    {
        Vector2 centerPoint = new Vector2(size / 2, size / 2);

        float pointHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        pointX = (int)point.x;
        pointY = (int)point.y;

        float sampleX;
        float sampleY;

        float halfHeight = size / 2f;
        float halfWidth = size / 2f;

        for (int i = 0; i < octaves; i++)
        {
            sampleX = (pointX - halfWidth) / scale * frequency + offsetX;
            sampleY = (pointY - halfHeight) / scale * frequency + offsetY;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            pointHeight += perlinValue * amplitude;

            frequency *= lacunarity;
            amplitude *= persistance;
        }

        float distanceFromEdge = Mathf.Max(Mathf.Abs(pointX / (float)size * 2 - 1), Mathf.Abs(pointY / (float)size * 2 - 1));

        return 1 - pointHeight - EdgeHeightMultiplier(distanceFromEdge);
    }

    float EdgeHeightMultiplier(float distanceFromEdge)
    {
        float heightMultiplier = Mathf.Pow(distanceFromEdge, fallOffStrength) / (Mathf.Pow(distanceFromEdge, fallOffStart) + Mathf.Pow(fallOffStart - fallOffStart * distanceFromEdge, fallOffStrength));

        return heightMultiplier;
    }

    Texture2D GenerateColorMap()
    {
        Texture2D colorTexture = new Texture2D(size, size);

        Color[] colorMap = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                for (int i = 0; i < regions.Length; i++)
                {
                    if (GetPointHeight(new Vector2(x, y)) >= regions[i].startHeight)
                    {
                        colorMap[y * size + x] = regions[i].color;
                    }
                }
            }
        }

        colorTexture.wrapMode = TextureWrapMode.Clamp;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.SetPixels(colorMap);
        colorTexture.Apply();

        return colorTexture;
    }
}

[System.Serializable]
public struct Region
{
    public string name;
    public float startHeight;
    public Color color;
}