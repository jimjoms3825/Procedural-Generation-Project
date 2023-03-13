using UnityEngine;
using UnityEngine.AI;

/*
 * James Bombardier
 * COMP 495
 * November 22nd, 2022
 * 
 * Class: Squad Factory
 * Description: Factory class which spawns a squad at a given position with a given number of enemies. 
 */

[CreateAssetMenu(fileName = "newSquadFactory")]
public class SquadFactory : ScriptableObject
{
    //The enemy prefabs to be instantiated at random. 
    [SerializeField] private GameObject[] enemiesToSpawn;

    //Spawns a squad with size enemies at position. 
    public void spawnSquad(Vector3 position, int size)
    {
        Squad squad = new GameObject("Squad").AddComponent(typeof(Squad)) as Squad;
        //For each enemy to be spawned. 
        for (int i = 0; i < size; i++)
        {
            GameObject go = Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)]);
            go.GetComponent<NavMeshAgent>().Warp(position);
            squad.addEnemy(go.GetComponent<Enemy>());
            go.GetComponent<Enemy>().squad = squad;
            go.transform.parent = squad.transform;
        }
    }
}
