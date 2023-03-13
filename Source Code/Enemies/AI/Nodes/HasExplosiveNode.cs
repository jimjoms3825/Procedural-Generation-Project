/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: HasExplosiveNode
 * Description: Checks to see if this enemy agent has explosives available
 * for use. 
 */
public class HasExplosiveNode : Node
{
    private Enemy enemy;
    public HasExplosiveNode(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        if(enemy.explosives > 0)
        {
            state = NodeStates.Success;
            return state;
        }
        state = NodeStates.Failure;
        return state;
    }

}
