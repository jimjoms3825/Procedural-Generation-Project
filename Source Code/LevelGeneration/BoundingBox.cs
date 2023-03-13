using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: BoundingBox
 * Description: A bounding box containing one or more colliders, and encompassing the bounds of a room. 
 */

public class BoundingBox : MonoBehaviour
{
    //The collection of colliders. 
    [SerializeField] private LinkedList<GameObject> colliders;
    //A collection of gameobjects collided with. 
    private LinkedList<GameObject> collisions;

    //A reference to the in-game layer. 
    private static int boundingBoxLayer = 10;

    private LayerMask groundMask;

    private void Awake()
    {
        groundMask = LayerMask.GetMask("Ground");
        //Instantiate the collider list. 
        colliders = new LinkedList<GameObject>();
        //Get the colliders which are children of the bounding box.
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<BoxCollider>())
            {
                colliders.AddLast(transform.GetChild(i).gameObject);
            }
        }
        //Instantiate the collisions list. 
        collisions = new LinkedList<GameObject>();
        //Throw warning when bounding box has no colliders. 
        if (colliders.Count <= 0)
        {
            Debug.LogWarning("No collider found in BoundingBox object. Destroying Bounding box.");
            Destroy(gameObject);
        }

        gameObject.layer = boundingBoxLayer;
        //Assign propper collider variables. 
        foreach(GameObject col in colliders)
        {
            col.GetComponent<BoxCollider>().isTrigger = true;
            col.layer = boundingBoxLayer;
        }

        //Rigidbody allows for collision detection during physics updates. 
        Rigidbody rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    //Adds colliding entities to the collisions list if not already present.
    private void OnTriggerStay(Collider other)
    {
        if (!colliders.Contains(other.gameObject) && !collisions.Contains(other.gameObject))
        {
            collisions.AddLast(other.gameObject);
        }
    }

    //Remove collider when no longer inside of bounds. 
    private void OnTriggerExit(Collider other)
    {
        if (!colliders.Contains(other.gameObject) && collisions.Contains(other.gameObject))
        {
            collisions.Remove(other.gameObject);
        }
    }

    //Returns whether there is a collision present in this gameobject. 
    public bool hasCollision()
    {
        if (collisions.Count == 0) { return false; }
        //Clean collider list. Because Unity doesnt register onTriggerExit when items are Destroyed.
        LinkedList<GameObject> newList = new LinkedList<GameObject>();
        //Remake the collisions list, culling null (destroyed) elements. 
        foreach (GameObject bb in collisions)
        {
            if (bb != null)
            {
                newList.AddLast(bb);
            }
        }
        collisions = newList;
        foreach (GameObject bb in collisions)
        {
            if(bb != null)
            {
                return true;
            }
        }
        return false;
    }

    //Returns the collective center of all box colliders in bounding box.
    public Vector3 getCenter()
    {
        Vector3 center = new Vector3();
        LinkedListNode<GameObject> collider = colliders.First;
        for (int i = 0; i < colliders.Count; i++)
        {
            center += collider.Value.transform.position;
            collider = collider.Next;
        }
        center.x = center.x / colliders.Count;
        center.z = center.z / colliders.Count;

        RaycastHit hit;
        Physics.Raycast(new Vector3(center.x, 300, center.z), Vector3.down, out hit, 400, groundMask);

        center.y = hit.point.y;
        return center;
    }
}
