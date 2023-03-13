using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: Grenade
 * Description: A throwable grenade used by the player. 
 */

public class Grenade : MonoBehaviour
{
    //Explosion prefab. 
    [SerializeField] private GameObject explosionEffectPrefab;

    //Variables for the grenade.
    [SerializeField] private float grenadeTime;
    [SerializeField] private float grenadeKnockBack;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float grenadeDamage;

    //Begins a threaded method on the first frame. 
    void Start()
    {
        StartCoroutine(cookGrenade());
    }
    
    //This method "pulls the pin" on the grenade, starting a timer that will detonate after a timer has expired. 
    private IEnumerator cookGrenade()
    {
        yield return new WaitForSeconds(grenadeTime);
        Instantiate(explosionEffectPrefab).transform.position = transform.position;
        SoundManager.GetInstance().playSoundAtPosition("event:/Explosion", gameObject);

        //Damage all nearby enemies. 
        Collider[] nearby = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider col in nearby)
        {
            if(col.GetComponent<Life>() != null)
            {
                col.GetComponent<Life>().dealDamage((1 - (Vector3.Distance(transform.position, col.transform.position) / explosionRadius)) * grenadeDamage);
            } else if(col.GetComponentInParent<Life>() != null)
            {
                col.GetComponentInParent<Life>().dealDamage((1 - (Vector3.Distance(transform.position, col.transform.position) / explosionRadius)) * grenadeDamage);
            }
            if(col.GetComponent<Rigidbody>() != null)
            {
                col.GetComponent<Rigidbody>().AddExplosionForce(grenadeKnockBack, transform.position, explosionRadius);
            } 
            else if(col.GetComponentInParent<Rigidbody>() != null)
            {
                col.GetComponentInParent<Rigidbody>().AddExplosionForce(grenadeKnockBack, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }
}
