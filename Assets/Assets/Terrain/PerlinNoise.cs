using UnityEditor.Search;
using UnityEngine;

public class PerlinNoise
{
    public static float[,] GeneratePerlinNoise(int mapWidth, int mapHeight, float noiseScale)
    {
        float[,] noiseFactor = new float[mapWidth,mapHeight];

        if (noiseScale <= 0)
        {
            noiseScale = 0.0001f;
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float xFactor = mapWidth / noiseScale;
                float yFactor = mapHeight / noiseScale;
                
                float noise = Mathf.PerlinNoise(xFactor, yFactor);
                noiseFactor[x, y] = noise;
            }
        }
        return noiseFactor;
    }
}
