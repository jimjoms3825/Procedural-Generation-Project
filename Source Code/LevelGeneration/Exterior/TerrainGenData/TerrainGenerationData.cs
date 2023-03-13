using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 21st, 2022
 * 
 * Class: TerrainGenerationData 
 * Description: A scriptable object that stores profiles for exterior terrain generation. 
 */

[CreateAssetMenu(fileName = "NewTerrainGenData")]
public class TerrainGenerationData : ScriptableObject
{
    //Serialized structs for painting. 
    [System.Serializable]
    public struct TextureSplatData
    {
        public int textureIndex;
        public float height; // Height at which to apply texture. 
        [Range(1, 20)]
        public float fade; // How large of a transition down is present. 
    }

    //Serialized struct for controlling heights based off of perlin noise. 
    [System.Serializable]
    public struct TerrainHeightSpline
    {
        public float height;
        public float scale;
    }

    //Noise scale. 
    public float scale;
    //Noise frequency. 
    public float frequency;
    //The number of octaves of noise generated. 
    public int octaves;
    //The reduction in scale per octave.
    public float octaveScaleWane;
    //The increase in noise frequency per octave.
    public float octaveFrequencyBoost;
    //Smoothing applied post-generation. 
    [Range(0f, 1f)]
    public float terrainSmoothingFactor;

    public TerrainHeightSpline[] heightSplines;
    public TextureSplatData[] textureData;
    public PoissonScatterTerrain[] scatterTerrain;
}
