using UnityEngine;
using UnityEngine.AI;

/*
 * James Bombardier
 * COMP 495
 * November 21st, 2022
 * 
 * Class: EnemySpawner 
 * Description: Spawns enemies at regular intervals. 
 */


public class EnemySpawner : MonoBehaviour
{
    private Squad spawnedSquad; // Squad all spawns become a member of. 

    [SerializeField] private int maxNumberOfEnemies; // The maximimum spawns.
    [SerializeField] private GameObject[] enemiesToSpawn; // List of spawnable enemy objects.
    [SerializeField] private float timeToSpawn = 5f; // Time in seconds between enemy spawns.
    private int lastSpawnFrame = 0;

    private void Awake()
    {
        //Instantiate squad. 
        spawnedSquad = new GameObject("Squad").AddComponent(typeof(Squad)) as Squad;
    }

    //Called once per frame. 
    private void LateUpdate()
    {
        if(spawnedSquad.enemiesInSquad.Count < maxNumberOfEnemies && lastSpawnFrame++ >= timeToSpawn / 60)
        {
            GameObject go = Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)]);
            go.GetComponent<NavMeshAgent>().Warp(transform.position);
            spawnedSquad.addEnemy(go.GetComponent<Enemy>());
            go.GetComponent<Enemy>().squad = spawnedSquad;
            go.transform.parent = spawnedSquad.transform;
            //Reset the timer. 
            lastSpawnFrame = 0;
        }
    }
}
