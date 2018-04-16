using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    #region Variables

    public int width;
    public int height;

    int pointX;
    int pointY;

    public float depth;
    public float scale;
    public float edgeStartDistance;
    public float lacunarity;
    public float persistance;
    public float octaves;

    #endregion

    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();

        terrain.terrainData = GenerateData(terrain.terrainData);
    }

    TerrainData GenerateData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        GenerateNoise();
        terrainData.SetHeights(0, 0, GenerateNoise());

        return terrainData;
    }

    float[,] GenerateNoise()
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

        for (int i = 0; i < octaves; i++)
        {
            sampleX = pointX / scale * frequency;
            sampleY = pointY / scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            pointHeight += perlinValue * amplitude;

            frequency *= lacunarity;
            amplitude *= persistance;
        }

        float heightMultiplier = 1f - (Vector2.Distance(point, centerPoint) / width * edgeStartDistance);

        //pointHeight = Mathf.InverseLerp(0f, 1f, pointHeight) * heightMultiplier;

        return pointHeight * heightMultiplier;
    }
}
