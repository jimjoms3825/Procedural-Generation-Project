/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: HasMedicRequestsNode
 * Description: Node for the medic behaviour tree which checks to see
 * if there are any open medic requests to fill within the agent's
 * squad. 
 */
public class HasMedicRequestsNode : Node
{
    private Enemy enemy;
    public HasMedicRequestsNode(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        if(enemy.squad.healRequests.Count > 0)
        {
            state = NodeStates.Success;
            return state;
        }
        state = NodeStates.Failure;
        return state;
    }
}
