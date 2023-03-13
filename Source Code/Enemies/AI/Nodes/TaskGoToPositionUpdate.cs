using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: 
 * Description: Sends the agent to a specified position. This node allows for 
 * interruptions in excecution. 
 */

public class TaskGoToPositionUpdate : Node
{
    private NavMeshAgent navMeshAgent;
    private Vector3 position;
    private string key;

    public TaskGoToPositionUpdate(string _key, BehaviorTree _tree) : base(_tree)
    {
        navMeshAgent = behaviorTree.GetComponent<NavMeshAgent>();
        key = _key;
    }

    public override NodeStates evaluate()
    {
        position = (Vector3)behaviorTree.blackBoard.getData(key);
        if (navMeshAgent != null && position != null)
        {
            navMeshAgent.destination = position;
        }
        //Returning success (not running) allows for other nodes to be excecuted. 
        state = NodeStates.Success;
        return state;
    }
}
