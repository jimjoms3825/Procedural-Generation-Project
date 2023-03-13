using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 22nd, 2022
 * 
 * Class: chanceToSpawn 
 * Description: Provides a gameobject with a chance to be destroyed on awake. 
 */

public class chanceToSpawn : MonoBehaviour
{
    //Chance to not be destroyed. 
    [SerializeField, Range(0, 1)] private float chanceToKeep = 1;
    private void Awake()
    {
        if(Random.Range(0, 1f) > chanceToKeep)
        {
            Destroy(gameObject);
        }
        else
        {
            //Remove component. 
            Destroy(this);
        }
    }
}
