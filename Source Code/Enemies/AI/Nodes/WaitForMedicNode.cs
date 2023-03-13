/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: WaitForMedicNode
 * Description: Has the agent wait in place for a medic. 
 */

public class WaitForMedicNode : Node
{
    private Life life;
    private float targetLifePercentage;
    public WaitForMedicNode(BehaviorTree _tree, float _targetLifePercentage) : base(_tree)
    {
        life = behaviorTree.GetComponent<Life>();
        targetLifePercentage = _targetLifePercentage;
    }

    public override NodeStates evaluate()
    {
        if(life == null)
        {
            state = NodeStates.Failure;
            return state;
        }

        float currentLifePercentage = (life.getLife() / life.getMaxLife());
        if (currentLifePercentage < targetLifePercentage)
        {
            state = NodeStates.Running;
        }
        else
        {
            state = NodeStates.Success;
        }

        return state;
    }
}
