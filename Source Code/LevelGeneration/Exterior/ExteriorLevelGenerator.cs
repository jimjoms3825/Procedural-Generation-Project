using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 21st, 2022
 * 
 * Class: ExteriorLevelGenerator
 * Description: Generates exterior levels.
 */

public class ExteriorLevelGenerator : MonoBehaviour
{
    //Keep a static reference to the instance of level generator. 
    public static ExteriorLevelGenerator instance;
    //base level prefab. 
    [SerializeField] private ExteriorLevel levelPrefab;
    //Keep reference to the teleporter. 
    [SerializeField] private PoissonScatterTerrain teleporter;
    [SerializeField] private SquadFactory squadFactory;

    private int groundMask;


    private void Awake()
    {
        //SINGLTON PATTERN.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("More than one LevelGenerators in scene! Deleting second instance.");
            Destroy(this);
        }
        groundMask = LayerMask.GetMask("Ground");
    }

    private void Start()
    {
        //Starts a threaded routine for level generation. 
        StartCoroutine(createExteriorLevel());
    }

    /*
     * Uses perlin noise and a terrain generation profile to create a procedurally generated environment. 
     */
    public IEnumerator createExteriorLevel()
    {
        //Instantiate level and give the system time to register the object in the physics system.
        ExteriorLevel newLevel = Instantiate(levelPrefab).GetComponent<ExteriorLevel>();
        yield return new WaitForFixedUpdate();

        //Assign a terrain variable. 
        Terrain terrain = newLevel.terrain;
        //Create a 2D array to store height variables from perlin noise sampling. 
        float[,] terrainHeights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);

        //Set a random seed. 
        Random.InitState(newLevel.SEED);
        //Create a seed for the Perlin noise offset based off of the random seed. 
        int perlinSeed = Random.Range(100, 100000);

        //Double for-loop to generate terrain heights through perlin noise sampling. 
        for (int i = 0; i < terrain.terrainData.heightmapResolution; i++)
        {
            for (int j = 0; j < terrain.terrainData.heightmapResolution; j++)
            {
                terrainHeights[i, j] = getNoise(perlinSeed + i, perlinSeed + j, newLevel.generationData);
            }
        }
        //Assign the heights to the terrain from the 2D array. 
        terrain.terrainData.SetHeights(0, 0, terrainHeights);

        //Allow the engine to update. 
        yield return new WaitForFixedUpdate();

        //Apply the textures to the terrain. 
        paintTerrain(terrain.terrainData, newLevel.generationData);
        yield return new WaitForFixedUpdate();
        //Perform smoothing on the terrain. 
        smoothTerrain(terrain.terrainData, newLevel.generationData.terrainSmoothingFactor);
        yield return new WaitForFixedUpdate();
        //Generate the scatter terrain. 
        generateLandscape(terrain.terrainData, newLevel.generationData);
        yield return new WaitForFixedUpdate();
        //Build the navmesh surface. 
        newLevel.navMeshObjects.AddFirst(terrain.gameObject);
        newLevel.buildNavMesh();
        //Spawn the enemy squads. 
        spawnSquads(terrain.terrainData);
        //Get the height for the player to spawn on and spawn the player.
        float playerSpawnHeight = terrain.terrainData.GetHeight(terrain.terrainData.heightmapResolution / 2, terrain.terrainData.heightmapResolution / 2) + 5;
        GameManager.player.transform.position = new Vector3(500, playerSpawnHeight, 500);
        //Flag the gamemanager to remove the transition canvas. 
        GameManager.instance.readyForDetransition = true;
    }

    /*
     * Samples perlin noise at a provided position given the scale and frequency of the 
     * passed generation data. Performs all octave checks with one call. Also applies the 
     * scale relative to the heightSlpines. 
     */
    private float getNoise(int x, int y, TerrainGenerationData genData)
    {
        float perlinValue = 0f;
        float scale = genData.scale;
        float frequency = genData.frequency;

        //Loop for each octave. 
        for (int i = 0; i < genData.octaves; i++) // 3 octave
        {
            float thisPerlin = Mathf.PerlinNoise(x * frequency, y * frequency);
            perlinValue += getScale(thisPerlin, genData.heightSplines) * scale;
            scale = scale / genData.octaveScaleWane;
            frequency = frequency * genData.octaveFrequencyBoost;
        }
        //Return the height at the given coordinate.
        return perlinValue;
    }

    /*
     * Adjusts the scale of a perlin noise result by applying height-splines set by the 
     * developer.
     */
    private float getScale(float perlinHeight, TerrainGenerationData.TerrainHeightSpline[] splines)
    {
        //Keep perlinHeight in bounds. 
        if(perlinHeight > 1)
        {
            perlinHeight = 1;
        }
        for (int i = 0; i < splines.Length; i++)
        {
            if (perlinHeight <= splines[i].height)
            {
                if (i > 0)  // not the first element
                {
                    float lerpPercentage = ((perlinHeight - splines[i - 1].height) / (splines[i].height - splines[i - 1].height));
                    float lerpedScale = Mathf.Lerp(splines[i - 1].scale, splines[i].scale, lerpPercentage);
                    if (lerpedScale >= 1)
                    {
                        Debug.Log(lerpedScale);
                    }
                    return lerpedScale * perlinHeight;

                }
                else // the first element
                {
                    float lerpPercentage = perlinHeight / splines[i].height;
                    float lerpedScale = Mathf.Lerp(0, splines[i].scale, lerpPercentage);
                    if (lerpedScale >= 1)
                    {
                        Debug.Log(lerpedScale);
                    }
                    return lerpedScale * perlinHeight;
                }
            }
            
        }
        return 1; // Hope to never get here.
    }

    /*
     * Paints the terrain by using nested for loops. Scans the terrain's 2D splatMap, assigning textures by
     * height and blending where necessary to create smooth transitions between layers. 
     */
    private void paintTerrain(TerrainData terrainData, TerrainGenerationData terrainGenerationData)
    {
        TerrainGenerationData.TextureSplatData[] texData = terrainGenerationData.textureData;
        //3D float array for strength of splatmap. 
        float[,,] splatMap = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        //Width of the terrain. 
        for(int i = 0; i < terrainData.alphamapWidth; i++)
        {
            //Length of the terrain. 
            for(int j = 0; j < terrainData.alphamapHeight; j++)
            {
                //For each layer to be applied.
                for (int k = 0; k < terrainGenerationData.textureData.Length; k++)
                {
                    //If the terrain's height is above the threshold to paint. 
                    if (terrainData.GetHeight(j, i) > terrainGenerationData.textureData[k].height)
                    {
                        splatMap[i, j, terrainGenerationData.textureData[k].textureIndex] = 1;
                    }
                    //If the terrain's height is above the threshold to fade paint. 
                    else if (terrainData.GetHeight(j, i) > terrainGenerationData.textureData[k].height - terrainGenerationData.textureData[k].fade)
                    {
                        splatMap[i, j, terrainGenerationData.textureData[k].textureIndex] = (terrainGenerationData.textureData[k].fade - 
                            (terrainGenerationData.textureData[k].height - terrainData.GetHeight(j, i))) / terrainGenerationData.textureData[k].fade;
                    }
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatMap);
    }

    /*
     * Smooths the terrain heights. 
     */
    private void smoothTerrain(TerrainData terrainData, float scaleFactor)
    {
        float[,] smoothedHeights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for(int i = 2; i < terrainData.heightmapResolution - 2; i++)
        {
            for (int j = 2; j < terrainData.heightmapResolution - 2; j++)
            {
                float average = 0; // Sample and store 5x5 neighbors
                for(int x = i - 2; x <= i + 2; x++)
                {
                    for (int y = j - 2; y <= j + 2; y++)
                    {
                        average += terrainData.GetHeight(x, y);
                    }
                }
                average = (average / 25);
                smoothedHeights[j, i] = (terrainData.GetHeight(i,j) + ((average - terrainData.GetHeight(i,j)) * scaleFactor)) / terrainData.heightmapResolution;
            } 
        }
        terrainData.SetHeights(0, 0, smoothedHeights);
    }

    private GameObject generateLandscape(TerrainData terrainData, TerrainGenerationData terrainGenerationData)
    {
        //Spawn Teleporter
        PoissonScatterTerrain teleporterInstance = Instantiate(teleporter);
        int mapRes = terrainData.heightmapResolution;
        int x = (mapRes / 2) + Random.Range(-mapRes / 3, mapRes / 3);
        int z = (mapRes / 2) + Random.Range(-mapRes / 3, mapRes / 3);
        float y = terrainData.GetHeight(x, z);
        flattenArea(x, z, 3, 20, terrainData);
        teleporterInstance.transform.position = new Vector3(x * terrainData.heightmapScale.x, y, z * terrainData.heightmapScale.z);

        LinkedList<PoissonScatterTerrain> scatterList = new LinkedList<PoissonScatterTerrain>();
        scatterList.AddFirst(teleporterInstance);
        //spawn stuff
        for (int i = 0; i < terrainGenerationData.scatterTerrain.Length; i++)
        {
            PoissonScatterTerrain toScatter = terrainGenerationData.scatterTerrain[i];
            int numToSpawn = toScatter.averageToSpawn + Random.Range(-toScatter.deviation, toScatter.deviation);
            int numSpawned = 0;

            while (numSpawned < numToSpawn)
            {
                bool valid = false;
                GameObject newObj = Instantiate(terrainGenerationData.scatterTerrain[i].gameObject);
                int attempts = 0;
                while (attempts++ < 20)
                {
                    x = Random.Range(0, terrainData.heightmapResolution);
                    z = Random.Range(0, terrainData.heightmapResolution);
                    y = terrainData.GetHeight(x, z);

                    if(y > terrainGenerationData.scatterTerrain[i].minHeight && y < terrainGenerationData.scatterTerrain[i].maxHeight)
                    {
                        newObj.transform.position = new Vector3(x * terrainData.heightmapScale.x, y, z * terrainData.heightmapScale.z);
                        valid = true;
                        foreach (PoissonScatterTerrain ter in scatterList)
                        {
                            if (Vector3.Distance(newObj.transform.position, ter.transform.position) < ter.requiredDistance + toScatter.requiredDistance)
                            {
                                valid = false;
                                break;
                            }
                        }
                        if(!valid) { break; }
                    }
                    
                    break;
                }
                if (valid)
                {
                    scatterList.AddLast(newObj.GetComponent<PoissonScatterTerrain>());
                }
                else
                {
                    Destroy(newObj);
                }
                numSpawned++;
            }
        }
        GameObject parent = new GameObject("Scatter Terrain");
        foreach(PoissonScatterTerrain scatter in scatterList)
        {
            GameObject go = Instantiate(scatter.prefabs[Random.Range(0, scatter.prefabs.Length)]);
            go.transform.position = scatter.transform.position;
            go.transform.position -= Vector3.down * scatter.sinkValue;
            go.transform.Rotate(0, Random.Range(0, 360), 0);
            go.transform.parent = parent.transform;
            Destroy(scatter.gameObject);
        }
        return parent;
    }

    private void spawnSquads(TerrainData td)
    {
        int numberOfSquads = Random.Range(7, 15);
        for(int i = 0; i < numberOfSquads; i++)
        {
            int x = (int)(Random.Range(0.3f, 0.7f) * 1000);
            int y = (int)(Random.Range(0.3f, 0.7f) * 1000);

            RaycastHit hit;
            Physics.Raycast(new Vector3(x, 300, y), Vector3.down, out hit, 400, groundMask);

            Vector3 newPos = new Vector3(x, hit.point.y + 2, y);
            squadFactory.spawnSquad(newPos, Random.Range(2, 6));
        }

    }

    private void flattenArea(int x, int z, int radius, int smoothingRadius, TerrainData terrainData)
    {
        //Create a flat surface around teleporter. 
        float[,] newHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float targetHeight = terrainData.GetHeight(x, z) / terrainData.heightmapScale.y;
        //Set area around point to height.
        for (int i = x - radius; i < x + radius; i++)
        {
            if (i < 0 || i >= terrainData.heightmapResolution - 1) { continue; }
            for (int j = z - radius; j < z + radius; j++)
            {
                if (j < 0 || j >= terrainData.heightmapResolution - 1) { continue; }
                if(Vector2.Distance(new Vector2(x, z), new Vector2(i, j)) > radius) { continue; }
                newHeights[j, i] = targetHeight;
            }
        }
        terrainData.SetHeights(0, 0, newHeights);
    }
}


