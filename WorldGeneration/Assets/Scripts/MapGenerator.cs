using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    #region Variables

    public int width;
    public int height;
    public float depth;
    public float scale;

    #endregion

    void Update()
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
                noiseMap[x, y] = Mathf.PerlinNoise((float)x / width * scale, (float)y / height * scale);
            }
        }

        return noiseMap;
    }
}
