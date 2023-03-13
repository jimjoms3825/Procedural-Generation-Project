using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 22nd, 2022
 * 
 * Class: InteriorLevelGenerator
 * Description: Generates an interior level using a procedural threaded approach. The algorithm
 * generates a critical path from the entrance to the exit, and then generates branch paths based on the 
 * values provided to the class. 
 */

public class InteriorLevelGenerator : MonoBehaviour
{
    //Critical path variables.
    [SerializeField] private int critPathLengthMin = 5;
    [SerializeField] private int critPathLengthMax = 15;
    //Variables controlling the minimum and maximummbranch length. 
    [SerializeField] private int branchLengthMin = 3;
    [SerializeField] private int branchLengthMax = 7;
    //The maximum and mimum total number of spawned rooms. 
    [SerializeField] private int roomQuantityMin = 20;
    [SerializeField] private int roomQuantityMax = 40;

    //A reference to the generator. 
    private static InteriorLevelGenerator instance;
    //A collection of spawnable room prefabs. 
    [SerializeField] private Room[] rooms;
    //Potential rooms for the teleporter.
    [SerializeField] private Room[] endRooms;
    //The prefab level asset. 
    [SerializeField] private InteriorLevel levelPrefab;
    //The instantiated exit room. 
    [SerializeField] private Room exitRoom;

    //A squad factory for spawning. 
    [SerializeField] private SquadFactory squadFactory;

    //The output reference. 
    public Level generatedLevel;
    //Bool flag for when the generation has been completed. 
    public bool finished;

    private void Awake()
    {
        //Singleton Pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("More than one LevelGenerators in scene! Deleting second instance.");
            Destroy(this);
        }
    }

    //Starts the generation thread before the first update is called after instantiation. 
    private void Start()
    {
        StartCoroutine(CreateInteriorLevel());
    }

    //Alligns the given doors of two rooms. 
    private void alignDoors(Room r1, Door d1, Room r2, Door d2)
    {
        //Calculate the rotation.
        float rotationNeeded = 180 - (d1.transform.eulerAngles.y - d2.transform.eulerAngles.y);
        r1.transform.Rotate(0, rotationNeeded, 0);

        Vector3 doorOffset = r1.transform.position - d1.getCentre(); // Local offset of the door in the room to be moved.
        r1.transform.position = d2.getCentre() + doorOffset; // Moves the room to the door.

        //give a slight space between rooms so meshes dont overlap.
        Vector3 offset = d2.transform.rotation.eulerAngles.normalized * 0.5f;
        r1.transform.position += new Vector3(offset.x, 0, offset.z);
    }

    //Checks each room to see if any bounding boxes have collisions. 
    private bool checkCollisions(LinkedList<Room> roomList)
    {
        foreach (Room r in roomList)
        {
            if (r.boundingBox.hasCollision())
            {
                return true;
            }
        }
        return false;
    }

    //Checks a newly spawned room to see if it or any other spawned room have collisions. 
    private bool checkCollisions(BoundingBox newRoomBox, LinkedList<Room> roomList)
    {
        if (newRoomBox.hasCollision())
        {
            return true;
        }
        foreach(Room r in roomList)
        {
            if (r.boundingBox.hasCollision())
            {
                return true;
            }
        }
        return false;
    }


    //Had to switch from recursion to fit in with unity's Asynch pattern and allow for collision detection.
    
    //Creates a level made of rooms connected through a series of doors. 
     
    public IEnumerator CreateInteriorLevel()
    {
        //Create the level and set the seed. 
        InteriorLevel newLevel = Instantiate(levelPrefab);
        Random.InitState(newLevel.SEED);

        //Generate the number of rooms and critical path length. 
        int roomQuantity = Random.Range(roomQuantityMin, roomQuantityMax);
        int criticalPathLength = Random.Range(critPathLengthMin, critPathLengthMax) - 1;

        //Make CriticalPath.
        do
        {
            //Whether the rooms has collision.
            bool roomCollision = true;
            //The number of times a new room has been attempted. 
            int roomAttempts = 0;
            //The previously generated room in sequence. 
            Room lastRoom = newLevel.getRooms().Last.Value;
            //A random door possessed by the previous room. 
            Door lastRoomDoor = lastRoom.getRandomAvailableDoorDoor();

            //Spawn a room and attempt to fit it to the previous room. 
            while (roomAttempts++ < 5) 
            {
                //Re-assign fresh variables for loop. 
                roomCollision = true;
                lastRoom = newLevel.getRooms().Last.Value;
                lastRoomDoor = lastRoom.getRandomAvailableDoorDoor();

                //Creates a new room.
                Room newRoom;
                //If not end room.
                if (newLevel.getRooms().Count < criticalPathLength)
                {
                    newRoom = Instantiate(rooms[Random.Range(0, rooms.Length)], newLevel.gameObject.transform);
                }
                //Else spawn end room. 
                else
                {
                    newRoom = Instantiate(endRooms[Random.Range(0, endRooms.Length)], newLevel.gameObject.transform);
                }
                //Grab a door from the spawned room. 
                Door newRoomDoor = newRoom.getRandomAvailableDoorDoor();
                //Count the number of times doors from this room have been tried. 
                int doorAttempts = 0;

                //Attempt alligning doors loop. Max of 5 attempts before backtracking occurs.  
                while (doorAttempts++ <= 5 && roomCollision) // Try to get a good door allignment. More performant than creating many rooms.
                {
                    alignDoors(newRoom, newRoomDoor, lastRoom, lastRoomDoor);
                    yield return new WaitForFixedUpdate(); // allows collision update.
                    roomCollision = checkCollisions(newRoom.boundingBox, newLevel.getRooms());
                    if (roomCollision) //Repick doors if placement failed.
                    {
                        newRoomDoor = newRoom.getRandomAvailableDoorDoor();
                        lastRoomDoor = lastRoom.getRandomAvailableDoorDoor();
                    }
                }
                //If no non-colliding door was found, destroy the room and repeat the loop. 
                if (roomCollision)
                {
                    Destroy(newRoom.gameObject);
                    continue;
                }
                else // Placement successful
                {
                    Door.connectDoors(newRoomDoor, lastRoomDoor);
                    newLevel.addRoom(newRoom);
                    break;
                }
            }
            //Allow for physics update. 
            yield return new WaitForFixedUpdate();

            //at this point if there is a collision or used all room attempts, then this room is no good to place other rooms on. Backtrack.
            if (checkCollisions(newLevel.getRooms()) || roomAttempts >= 5)
            {
                for(int i = 0; i < Random.Range(1, 5); i++)
                { 
                    Room badRoom = newLevel.getRooms().Last.Value;
                    if (badRoom.cannotDelete) { break; } // dont break past immutable room. Includes portal and entry rooms. 
                    newLevel.removeRoom(badRoom);
                    Destroy(badRoom.gameObject);
                }

            }

        } while (newLevel.getRooms().Count <= criticalPathLength); // Keep trying to make a critical path until length has been reached without collision.

        //Assign end room flag when critical path length met. 
        newLevel.getRooms().Last.Value.cannotDelete = true;

        //Now creating the branches. No rooms generated before this point can be changed now.
        while (newLevel.getRooms().Count < roomQuantity)
        {
            //Generate a length for the current branch. 
            int newBranchLength = Random.Range(branchLengthMin, branchLengthMax);
            //Keep track of the number of rooms in this branch. 
            int branchCount = 0;

            //Create new rooms until the branch length is met. 
            do
            {
                bool roomCollision = true;
                int roomAttempts = 0;
                Room lastRoom = newLevel.getRooms().Last.Value;

                Door lastRoomDoor = lastRoom.getRandomAvailableDoorDoor();
                while (roomAttempts++ < 5) // Tries different new rooms.
                {
                    roomCollision = true;
                    lastRoom = newLevel.getRooms().Last.Value;
                    if (branchCount == 0) // Get a random room instead if this is the start of a new branch.
                    {
                        lastRoom = newLevel.getRandomRoom();
                    }
                    lastRoomDoor = lastRoom.getRandomAvailableDoorDoor();
                    Room newRoom = Instantiate(rooms[Random.Range(0, rooms.Length)], newLevel.gameObject.transform);
                    Door newRoomDoor = newRoom.getRandomAvailableDoorDoor();
                    int doorAttempts = 0;


                    while (doorAttempts++ <= 5 && roomCollision) // Try to get a good door allignment. More performant than creating many rooms.
                    {
                        alignDoors(newRoom, newRoomDoor, lastRoom, lastRoomDoor);
                        yield return new WaitForFixedUpdate();
                        roomCollision = checkCollisions(newRoom.boundingBox, newLevel.getRooms());
                        if (roomCollision) //Repick doors if placement failed.
                        {
                            newRoomDoor = newRoom.getRandomAvailableDoorDoor();
                            lastRoomDoor = lastRoom.getRandomAvailableDoorDoor();
                            continue;
                        }

                    }
                    //  handle no possible non-collision found
                    if (roomCollision)
                    {
                        Destroy(newRoom.gameObject);
                        continue;
                    }
                    else // Placement successful
                    {
                        Door.connectDoors(newRoomDoor, lastRoomDoor);
                        newLevel.addRoom(newRoom);
                        branchCount++;
                        break;
                    }

                }

                //Allow the physics system to update. 
                yield return new WaitForFixedUpdate();
                //at this point if there is a collision or used all room attempts, then this room is no good to place other rooms on. Backtrack.
                if (checkCollisions(newLevel.getRooms()) || roomAttempts >= 5)
                {
                    for (int i = 0; i < Random.Range(1, branchCount); i++)
                    {
                        Room badRoom = newLevel.getRooms().Last.Value;
                        if (badRoom.cannotDelete) { break; } // dont break immutable room.
                        newLevel.removeRoom(badRoom);
                        branchCount--;
                        Destroy(badRoom.gameObject);
                    }
                }
            } while (branchCount < newBranchLength && newLevel.getRooms().Count < roomQuantity);
        }

        //Allow all rooms to clean themselves up.
        foreach (Room r in newLevel.getRooms())
        {
            r.finalize();
            yield return new WaitForFixedUpdate();
        }

        //Build the navmesh.
        newLevel.navMeshObjects.AddFirst(newLevel.gameObject);
        newLevel.buildNavMesh();

        //Place the player. 
        if (GameManager.player != null)
        {
            GameManager.player.transform.position = new Vector3(0, 0, 0);
        }

        //Spawn enemies in all mutable rooms. 
        foreach (Room r in newLevel.getRooms())
        {
            if(r.cannotDelete == false)
            {
                squadFactory.spawnSquad(r.boundingBox.getCenter(), Random.Range(1, 4));
                yield return new WaitForFixedUpdate();
            }
        }

        //Remove bounding boxes. 
        foreach (Room r in newLevel.getRooms())
        {
            Destroy(r.boundingBox.gameObject);
            yield return new WaitForFixedUpdate();
        }

        //Finish generation. 
        finished = true;
        yield return null;
        if(GameManager.instance != null)
        {
            GameManager.instance.readyForDetransition = true;
        }
    }
}
