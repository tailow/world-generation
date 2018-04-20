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
    int mapSize;
    int amountOfChunksPerLine;

    const int chunkSize = 97;

    public Region[] regions;

    public GameObject chunkPrefab;

    public Transform terrainParent;

    float minPointHeight;
    float maxPointHeight;

    public float depth;
    public float scale;
    public float lacunarity;
    public float fallOffStrength;
    public float fallOffStart;

    [Range(0, 1)]
    public float persistance;

    public int octaves;
    public int seed;
    public int amountOfChunks;

    [Range(0, 6)]
    public int levelOfDetail;

    public bool autoUpdate;

    public AnimationCurve meshHeightCurve;

    #endregion

    void Start()
    {
        GenerateChunks();

        mapSize = (chunkSize - 1) * amountOfChunksPerLine;

        Debug.DrawLine(new Vector3(chunkSize / 2, 0, chunkSize / 2), new Vector3(chunkSize / 2, 100, chunkSize / 2), Color.red, 10f);
        Debug.Log(chunkSize / 2);
    }

    public void GenerateChunks()
    {
        mapSize = (chunkSize - 1) * amountOfChunksPerLine;

        amountOfChunksPerLine = (int)Mathf.Sqrt(amountOfChunks);

        System.Random rng = new System.Random(seed);

        offsetX = rng.Next(-50000, 50000);
        offsetY = rng.Next(-50000, 50000);

        if (terrainParent.childCount != amountOfChunks)
        {
            for (int i = 0; i < terrainParent.childCount; i++)
            {
                DestroyImmediate(terrainParent.GetChild(i).gameObject);
            }
        }

        if (terrainParent.childCount == 0)
        {
            for (int y = 0; y < amountOfChunksPerLine; y++)
            {
                for (int x = 0; x < amountOfChunksPerLine; x++)
                {
                    GameObject chunk = Instantiate(chunkPrefab, new Vector3(x * (chunkSize - 1), 0, y * (chunkSize - 1)), Quaternion.identity);

                    chunk.transform.parent = terrainParent;
                }
            }
        }

        for (int i = 0; i < terrainParent.childCount; i++)
        {
            GenerateTerrain(terrainParent.GetChild(i).gameObject);
        }
    }

    public void DeleteChunks()
    {
        if (terrainParent.childCount > 0)
        {
            for (int i = 0; i < amountOfChunks; i++)
            {
                DestroyImmediate(terrainParent.GetChild(0).gameObject);
            }
        }
    }

    public void GenerateTerrain(GameObject chunk)
    {
        MeshFilter terrainMesh = chunk.GetComponent<MeshFilter>();

        terrainMesh.mesh = MeshGenerator.GenerateTerrainMesh(GenerateNoiseMap(chunk), meshHeightCurve, levelOfDetail, depth).CreateMesh();

        terrainMesh.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = GenerateColorMap(chunk);
    }

    float[,] GenerateNoiseMap(GameObject chunk)
    {
        float[,] noiseMap = new float[chunkSize, chunkSize];

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                noiseMap[x, y] = GetPointHeight(new Vector2(x, y), chunk);
            }
        }

        return noiseMap;
    }

    float GetPointHeight(Vector2 point, GameObject chunk)
    {
        float pointHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        pointX = (int)point.x + (int)chunk.transform.position.x;
        pointY = (int)point.y - (int)chunk.transform.position.z;

        float sampleX;
        float sampleY;

        float maxPossibleHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            sampleX = pointX / scale * frequency + offsetX;
            sampleY = pointY / scale * frequency + offsetY;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            pointHeight += perlinValue * amplitude;

            maxPossibleHeight += amplitude;

            frequency *= lacunarity;
            amplitude *= persistance;
        }

        pointHeight = pointHeight / maxPossibleHeight;

        return pointHeight - GenerateFallOffMap(new Vector2(pointX, pointY), new Vector2(mapSize / 2, 0));
    }

    float GenerateFallOffMap(Vector2 point, Vector2 center)
    {
        float distanceFromCenter = Vector2.Distance(center, point);

        float currentAlpha = 1;
        float a = fallOffStrength;
        float b = fallOffStart;


        if (1 - (distanceFromCenter / chunkSize) >= 0)
        {
            currentAlpha = 1 - (distanceFromCenter / chunkSize);
        }

        else
        {
            currentAlpha = 0;
        }

        float fallOffMultiplier = Mathf.Pow(currentAlpha, -a) / (Mathf.Pow(currentAlpha, -a) + Mathf.Pow(b - b * currentAlpha, -a));

        return fallOffMultiplier;
    }

    Texture2D GenerateColorMap(GameObject chunk)
    {
        Texture2D colorTexture = new Texture2D(chunkSize, chunkSize);

        Color[] colorMap = new Color[chunkSize * chunkSize];

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int i = 0; i < regions.Length; i++)
                {
                    if (GetPointHeight(new Vector2(x, y), chunk) >= regions[i].startHeight)
                    {
                        colorMap[y * chunkSize + x] = regions[i].color;
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