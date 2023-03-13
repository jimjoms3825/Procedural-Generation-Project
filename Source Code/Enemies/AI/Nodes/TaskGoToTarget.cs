using UnityEngine;
using UnityEngine.AI;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: TaskGoToTarget
 * Description: Sends the agent to the target specified. The node will
 * continue to run until the agent is sufficiently close to the destination
 * target. 
 */
public class TaskGoToTarget : Node
{
    private NavMeshAgent navMeshAgent;
    private Vector3 position;
    private string key;

    public TaskGoToTarget(string _key, BehaviorTree _tree) : base(_tree)
    {
        navMeshAgent = behaviorTree.GetComponent<NavMeshAgent>();
        key = _key;
    }

    public override NodeStates evaluate()
    {
        position = (Vector3)behaviorTree.blackBoard.getData(key);
        if (navMeshAgent != null && position != null)
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                return NodeStates.Success;
            }
            navMeshAgent.destination = position;

            if (Vector3.Distance(navMeshAgent.transform.position, navMeshAgent.destination) < 0.5f)
            {
                state = NodeStates.Success;
                return state;
            }
        }

        state = NodeStates.Running;
        return state;
    }

}
