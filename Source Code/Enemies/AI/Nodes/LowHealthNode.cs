/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: LowHealthNode
 * Description: Alerts he behavior tree that the health of the agent is low.
 */

public class LowHealthNode : Node
{
    private float triggerPercentage;
    private Life life;

    public LowHealthNode(BehaviorTree _tree, float _triggerPercentage) : base(_tree) 
    {
        triggerPercentage = _triggerPercentage;
        life = behaviorTree.GetComponent<Life>();
    }

    public override NodeStates evaluate()
    {
        if(life == null)
        {
            state = NodeStates.Failure;
            return state;
        }

        float currentLifePercentage = (life.getLife() / life.getMaxLife());
        if(currentLifePercentage <= triggerPercentage)
        {
            state = NodeStates.Success;
        }
        else
        {
            state = NodeStates.Failure;
        }

        return state;
    }
}
