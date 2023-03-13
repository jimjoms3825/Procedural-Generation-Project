using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: DestroyAfterSeconds
 * Description: Script which uses a threaded routine to destroy the gameobject it is attached to after a given number of seconds. 
 */
public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float secondsToDestroy;
    private void Awake()
    {
        StartCoroutine(destroyAfterSeconds(secondsToDestroy));
    }

    private IEnumerator destroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
    
}
