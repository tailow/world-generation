using UnityEngine;
using UnityEngine.UI;

public class NoiseGenerator : MonoBehaviour
{
    #region Variables

    public float noiseSize = 1.0f;
    public float noiseX;
    public float noiseY;
    public int noiseWidth = 100;
    public int noiseHeight = 100;

    private Texture2D noiseTexture;
    private Color[] pixelColorArray;
    private Image textureImage;

    #endregion

    void Start()
    {
        textureImage = GetComponent<Image>();
        noiseTexture = new Texture2D(noiseWidth, noiseHeight);
        pixelColorArray = new Color[noiseTexture.width * noiseTexture.height];
        textureImage.material.mainTexture = noiseTexture;
    }

    void Update()
    {
        GenerateNoise();
    }

    void GenerateNoise()
    {
        for (float y = 0.0f; y < noiseTexture.height; y++)
        {
            for (float x = 0.0f; x < noiseTexture.width; x++)
            {
                float xPos = noiseX + x / noiseTexture.width * noiseSize;
                float yPos = noiseY + y / noiseTexture.height * noiseSize;

                float pixelColor = Mathf.PerlinNoise(xPos, yPos);
                pixelColorArray[(int)y * noiseTexture.width + (int)x] = new Color(pixelColor, pixelColor, pixelColor);
            }
        }

        noiseTexture.SetPixels(pixelColorArray);
        noiseTexture.Apply();
    }
}
