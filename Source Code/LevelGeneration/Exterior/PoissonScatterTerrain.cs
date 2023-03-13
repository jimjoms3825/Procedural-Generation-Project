using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 22nd, 2022
 * 
 * Class: PoissonScatterTerrain
 * Description: A class for scatter terrain used in overworld levels.
 */

public class PoissonScatterTerrain : MonoBehaviour
{
    //One or more potential objects to populate the terrain piece.
    public GameObject[] prefabs;
    //The minimum and maximum heights at which the object can spawn.
    [Range(0, 600)]
    public float minHeight = 0;
    [Range(0, 600)]
    public float maxHeight = 600;
    //The number of these which should attempt to spawn in a map. 
    public int averageToSpawn = 20;
    //The deviation in spawn number. 
    public int deviation = 20;
    //The minimum distance required between this object and any other scatter terrain. 
    public float requiredDistance;
    //Depresses the object into the terrain by this amount. Avoids floating objects. 
    public float sinkValue;
}
