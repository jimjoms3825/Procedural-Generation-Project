using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: GetHealPositionNode
 * Description: A node for the medic behavior tree which sends them
 * to the closest open heal request. 
 */
public class GetHealPositionNode : Node
{
    private Enemy enemy;
    public GetHealPositionNode(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        if(enemy.squad.healRequests == null || enemy.squad.healRequests.Count < 1)
        {
            state = NodeStates.Failure;
            return state;
        }
        Enemy closestHealRequest = enemy.squad.healRequests.First.Value;
        //Loop through to get the closest heal request.
        foreach(Enemy otherEnemy in enemy.squad.healRequests)
        {
            if(Vector3.Distance(enemy.transform.position, otherEnemy.transform.position) <
                Vector3.Distance(enemy.transform.position, closestHealRequest.transform.position))
            {
                closestHealRequest = otherEnemy;
            }
        }
        Vector3 offset = (closestHealRequest.transform.position - behaviorTree.transform.position);
        offset.Normalize();// stay 2 meters away from target
        behaviorTree.blackBoard.addData("Heal Position", closestHealRequest.transform.position - offset * 2);
        behaviorTree.blackBoard.addData("Heal Enemy", closestHealRequest);
        state = NodeStates.Success;
        return state;
    }
}
