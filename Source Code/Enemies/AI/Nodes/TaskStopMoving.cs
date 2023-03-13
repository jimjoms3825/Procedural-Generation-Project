using UnityEngine.AI;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: TaskStopMoving
 * Description: Clears the target destination of the enemy's blackboard, 
 * halting movement. 
 */
public class TaskStopMoving : Node
{
    private NavMeshAgent navMeshAgent;

    public TaskStopMoving(BehaviorTree _tree) : base(_tree)
    {
        navMeshAgent = behaviorTree.GetComponent<NavMeshAgent>();
    }

    public override NodeStates evaluate()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.destination = behaviorTree.transform.position;
        }
        state = NodeStates.Success;
        return state;
    }
}
