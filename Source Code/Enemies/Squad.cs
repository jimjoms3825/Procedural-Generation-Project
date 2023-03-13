using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 22nd, 2022
 * 
 * Class: Squad
 * Description: Represents a group of enemies who can communicate amongst each other. The enemies in a squad 
 * share a common blackboard for AI behaviours and can make simple requests of each other situationally. 
 */

public class Squad : MonoBehaviour
{

    public LinkedList<Enemy> enemiesInSquad; // Collection of enemies. 
    public LinkedList<Enemy> healRequests; // Heal requests when medic is in squad. 

    public BlackBoard blackBoard; // Squad's common blackboard. 

    //Initializes the squad. 
    private void Awake()
    {
        enemiesInSquad = new LinkedList<Enemy>();
        healRequests = new LinkedList<Enemy>();
        blackBoard = new BlackBoard();
    }

    //The returned bool indicates whether the AI should call the request out.
    public bool callMedic(Enemy enemy)
    {
        if (healRequests.Contains(enemy)) { return false; }
        healRequests.AddLast(enemy);
        return true;
    }

    //Removes the request for healing from  the collection. 
    public void releaseMedicRequest(Enemy enemy)
    {
        healRequests.Remove(enemy);
    }

    //Returns true if any medic enemies are present in squad. 
    public bool hasMedics()
    {
        foreach(Enemy enemy in enemiesInSquad)
        {
            if (enemy.isMedic)
            {
                return true;
            }
        }
        return false;
    }

    //Adds an enemy to the squad if not already a member. 
    public void addEnemy(Enemy enemy)
    {
        if (!enemiesInSquad.Contains(enemy))
        {
            enemiesInSquad.AddLast(enemy);
        }
    }

    //Removes an enemy from the squad. Also clears heal requests by that enemy. 
    public void removeEnemy(Enemy enemy)
    {
        enemiesInSquad.Remove(enemy);
        healRequests.Remove(enemy);
    }

}
