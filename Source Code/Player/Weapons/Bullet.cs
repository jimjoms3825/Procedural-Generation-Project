using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Item tags to be omitted from collision affecting. 
    [SerializeField] private string[] ignoreTags;

    //Types of bullets. 
    public enum BulletTypes { Ballistic, Energy, Explosive, Shell }
    //Type of this bullet. 
    public BulletTypes bulletType;

    //The damage done when the bullet hits a target. 
    public float damage;
    //The particle effect when the bullet hits. (Used by explosives). 
    public GameObject hitEffect;

    //Stores whether a collision has taken place. 
    private bool hasCollided = false;

    //CAlled every physics update. 
    private void FixedUpdate()
    {
        RaycastHit hit;
        //Check if the bullet will have any collisions in the next physics update. 
        Physics.Raycast(transform.position, GetComponent<Rigidbody>().velocity, out hit, 5);
        if (hitEffect != null && hit.collider != null)
        {
            //Check each collision to see if it should be ignored 
            foreach(string s in ignoreTags)
            {
                //If tag should be ignored, return. 
                if(hit.collider.tag.Equals(s))
                {
                    return;
                }
            }
            //If collision is not ignored, create hiteffect. 
            GameObject effect = Instantiate(hitEffect);
            Vector3 hitPos = transform.position;
            if (hit.point != null)
            { 
                hitPos = hit.point;
            }
            effect.transform.position = hitPos;
        }
    }

    //When the bullet's trigger enters another object, 
    private void OnTriggerEnter(Collider other)
    {
        //Dont react to other triggers. 
        if (other.isTrigger)
        {
            return;
        }
        //Check to see if other collider should be ignored. 
        if(ignoreTags != null)
        {
            foreach(string s in ignoreTags)
            {
                if (other.gameObject.tag.Equals(s))
                {
                    return;
                }
            }
        }

        //If bullet is explosive. 
        if (bulletType == BulletTypes.Explosive && !hasCollided)
        {
            //Create explosion. 
            LinkedList<int> lifeList = new LinkedList<int>();
            SoundManager.GetInstance().playSoundAtPosition("event:/Explosion", gameObject);
            Collider[] nearby = Physics.OverlapSphere(transform.position, 3);
            //Damage all nearby entities with a LIFE script. 
            foreach (Collider col in nearby)
            {
                if (col.GetComponent<Life>() != null && !lifeList.Contains(col.GetComponent<Life>().GetInstanceID())) 
                {
                    lifeList.AddLast(col.GetComponent<Life>().GetInstanceID()); // Only attack each life once in case of multiple colliders.
                    col.GetComponent<Life>().dealDamage(damage);
                }
            }
            //Mark the explosive as having collided. 
            hasCollided = true;
        }
        //Non explosive bullet. 
        else
        {
            if(other.GetComponent<Life>() != null)
            {
                other.GetComponent<Life>().dealDamage(damage);
            }
        }
        Destroy(gameObject);
    }

    //Cull bullets too far away from the player. 
    private void LateUpdate()
    {
        if(Vector3.Distance(transform.position, GameManager.player.transform.position) > 200)
        {
            Destroy(gameObject);
        }        
    }
}
