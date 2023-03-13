using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: Room
 * Description: A room used for generation of interior levels. Rooms are connected to other
 * rooms through doors, and have bounding boxes which will prevent other rooms from generating inside of them. 
 */

public class Room : MonoBehaviour
{
    //Doors of the room which can be used for generation. 
    public Door[] doors;
    //A bounding box object (containing one or more colliders). 
    public BoundingBox boundingBox;
    //A boolean flag which removes the potential for deletion and spawning. 
    public bool cannotDelete;

    //Gets a random and available door from the doors collection.
    public Door getRandomAvailableDoorDoor()
    {
        Door openDoor;
        int attempts = 0;
        //Check a maximum of 10 times. 
        while(attempts++ < 10)
        {
            openDoor = doors[Random.Range(0, doors.Length)];
            if (openDoor.available)
            {
                return openDoor;
            }
        }

        //If a door cannot be found, default to the result of the getFirstAvailableDoor() method. 
        return getFirstAvailableDoor();
    }

    //Checks to ensure that at least one door in the collection is available. 
    public bool hasAvailableDoors()
    {
        for(int i = 0; i < doors.Length; i++)
        {
            if (doors[i].available)
            {
                return true;
            }
        }
        return false;
    }

    //Finalizes the object. Handle all finalization of doors as well. 
    public void finalize()
    {
        foreach(Door door in doors)
        {
            door.finalize();
        }
    }

    //Gets the first available door in the doors collection. Returns null if no doors found available.
    private Door getFirstAvailableDoor()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].available)
            {
                return doors[i];
            }
        }
        return null;
    }

}
