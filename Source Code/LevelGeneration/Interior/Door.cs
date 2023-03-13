using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 22nd, 2022
 * 
 * Class: Door
 * Description: A door for interior levels. Used in interior level generation to connect rooms together. 
 * Unused doors are replaced with walls on generation cleanup.
 */

public class Door : MonoBehaviour
{
    //Whether the door is open to connection. 
    public bool available = true;
    //Whether the door has been replaced by a wall. 
    public bool replaced;
    //Reference to connected door. 
    public Door connectedDoor;
    //Prefab of replacement wall. 
    public GameObject alternativeWall;
    //Whether the door is currently open. 
    public bool open;

    //The center of the door. 
    [SerializeField] private Transform centre;
    //The physical door (sans the frame, which is coupled with this object).
    [SerializeField] private GameObject door;

    //Enables or disables the door each physics update. 
    private void FixedUpdate()
    {
        if (door)
        {
            if (open)
            {
                door.SetActive(false);
            }
            else
            {
                door.SetActive(true);
            }
        }
    }

    //Connects two doors together. 
    public static void connectDoors(Door d1, Door d2)
    {
        if(d1.connectedDoor != null)
        {
            Debug.LogError("DOOR CONNECTION ERROR: Door " + d1.GetInstanceID() + " Is trying to be connected twice. Previous connection being overwritten.");
        }
        if(d2.connectedDoor != null)
        {
            Debug.LogError("DOOR CONNECTION ERROR: Door " + d2.GetInstanceID() + " Is trying to be connected twice. Previous connection being overwritten.");
        }

        d1.connectedDoor = d2;
        d1.available = false;
        d2.connectedDoor = d1;
        d2.available = false;
    }

    //Disconnects the passed door with it's connected door. 
    public static void disconnectDoors(Door d1)
    {
        if (d1.connectedDoor == null)
        {
            Debug.LogError("DOOR DISCONNECTION ERROR: Door " + d1.GetInstanceID() + " Is trying to be disconnected, but was never connected. This might cause errors.");
            return;
        }
        d1.connectedDoor.connectedDoor = null;
        d1.connectedDoor.available = true;
        d1.connectedDoor = null;
        d1.available = true;
    }

    //Getter for the center of the door (used for allignment). 
    public Vector3 getCentre()
    {
        return centre.position;
    }

    //Clean-up post generation. 
    public void finalize()
    {
        if (available)
        {
            //Destroy all child objects. 
            for(int i = transform.childCount - 1; i >=0 ; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            //instantiate and align replacement wall. 
            GameObject replacementwall = Instantiate(alternativeWall);
            replacementwall.transform.position = transform.position;
            replacementwall.transform.rotation = transform.rotation;
            replacementwall.transform.parent = transform;
        }
        else if(!connectedDoor.replaced && !replaced)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
               // Destroy(transform.GetChild(i).gameObject);
            }
            replaced = true;
        }
    }

    //Open the door. 
    public void OnTriggerStay(Collider other)
    {
        if(other.tag != "Player" && other.tag != "Enemy") { return; }
        open = true;
    }

    //Close the door. 
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player" && other.tag != "Enemy") { return; }
        open = false;
    }
}
