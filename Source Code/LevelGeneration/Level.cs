using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: Level
 * Description: An abstract class that provides a useful template for exterior and interior levels. 
 */
public abstract class Level : MonoBehaviour
{
    //The level's seed. 
    public int SEED;
    //A list of objects to build navmesh's on. 
    public LinkedList<GameObject> navMeshObjects;
    //A reference to the teleporter for easy access. 
    public Teleporter teleporterReference;

    protected void Awake()
    {
        //Set the seed. 
        SEED = Random.Range(-999999999, 999999999);
        //Instantiate the list. 
        navMeshObjects = new LinkedList<GameObject>();
    }

    //Builds the nav-mesh surface on each object in the navMeshObjects list. 
    public void buildNavMesh()
    {
        foreach(GameObject go in navMeshObjects)
        {
            NavMeshSurface surface = go.AddComponent<NavMeshSurface>();
            surface.layerMask = LayerMask.GetMask("Ground");
            surface.BuildNavMesh();
        }
    }
}
