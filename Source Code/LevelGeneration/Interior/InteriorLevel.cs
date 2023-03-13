using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 22nd, 2022
 * 
 * Class: InteriorLevel
 * Description: Represents a procedurally generated interior level. 
 */
public class InteriorLevel : Level
{
    //List of rooms instantiated.
    private LinkedList<Room> rooms;
    //Reference of the entrance room. 
    [SerializeField] private Room entrance;


    private void Awake()
    {
        base.Awake();
        rooms = new LinkedList<Room>();
        addEntrance(entrance);
    }

    //Returns the list of rooms. 
    public LinkedList<Room> getRooms()
    {
        return rooms;
    }

    //Adds a room to the rooms list. 
    public void addRoom(Room newRoom)
    {
        rooms.AddLast(newRoom);
    }

    //Removes a room from the list of rooms if present. 
    public void removeRoom(Room room)
    {
        if (!rooms.Contains(room)) { return; }
        rooms.Remove(room);
        foreach(Door d in room.doors)
        {
            if (!d.available)
            {
                Door.disconnectDoors(d);
            }
        }
    }

    //Adds the entrance room to the rooms list. 
    public void addEntrance(Room newEntrance)
    {
        rooms.AddFirst(newEntrance);
    }

    //Gets a random room from those present in the room list provided it has available doors. 
    public Room getRandomRoom()
    {
        int target = Random.Range(0, rooms.Count);
        var node = rooms.First;
        int index = 0;
        while(index++ < target)
        {
            node = node.Next;
        }
        if (node.Value.hasAvailableDoors())
        {
            return node.Value;
        }
        else
        { // uses recursion until room with available door found.
            return getRandomRoom();
        }
    }
}
