using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    #region Variables

    public int size;
    int width;
    int height;

    int offsetX;
    int offsetY;

    int pointX;
    int pointY;

    public float depth;
    public float scale;
    public float edgeStartDistance;
    public float lacunarity;
    public float persistance;

    public int octaves;
    public int seed;

    public bool autoUpdate;

    #endregion

    public void GenerateMap()
    {
        width = size;
        height = size;

        System.Random rng = new System.Random(seed);

        offsetX = rng.Next(-50000, 50000);
        offsetY = rng.Next(-50000, 50000);

        GenerateTerrain(GetComponent<MeshFilter>());
    }

    void GenerateTerrain(MeshFilter terrainMesh)
    {
        terrainMesh.sharedMesh = MeshGenerator.GenerateTerrainMesh(GenerateNoiseMap()).CreateMesh();
    }

    float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = GetPointHeight(new Vector2(x, y));
            }
        }

        return noiseMap;
    }

    float GetPointHeight(Vector2 point)
    {
        Vector2 centerPoint = new Vector2(width / 2, height / 2);

        float pointHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        pointX = (int)point.x;
        pointY = (int)point.y;

        float sampleX;
        float sampleY;

        float halfHeight = height / 2f;
        float halfWidth = width / 2f;

        for (int i = 0; i < octaves; i++)
        {
            sampleX = (pointX - halfWidth) / scale * frequency + offsetX;
            sampleY = (pointY - halfHeight) / scale * frequency + offsetY;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            pointHeight += perlinValue * amplitude;

            frequency *= lacunarity;
            amplitude *= persistance;
        }

        float heightMultiplier = 1f - (Vector2.Distance(point, centerPoint) / width * edgeStartDistance);

        return pointHeight * heightMultiplier * depth;
    }
}
