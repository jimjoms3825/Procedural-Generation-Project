using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 21st, 2022
 * 
 * Class: ExteriorLevel
 * Description: An overworld level using Unity's terrain engine. 
 */
public class ExteriorLevel : Level
{
    //Terrain reference.
    public Terrain terrain;
    //Assigned generation data.
    public TerrainGenerationData generationData;

    //Available generation data files. 
    [SerializeField] private TerrainGenerationData[] terrainGenerationTypes;

    private void Awake()
    {
        base.Awake();
        SEED = Random.Range(-999999999, 999999999);
        int i = Random.Range(0, terrainGenerationTypes.Length);

        //Randomly choose a terrain generation profile. 
        switch (i)
        {
            case 0:
                generationData = terrainGenerationTypes[0];
                break;
            case 1:
                generationData = terrainGenerationTypes[1];
                break;
            case 2:
                generationData = terrainGenerationTypes[2];
                break;
            case 3:
                generationData = terrainGenerationTypes[3];
                break;
        }
        terrain = GetComponentInChildren<Terrain>();
    }

}
