/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: MedicInSquad 
 * Description: Alerts the system if there is a medic in the agent's
 * squad. 
 */
public class MedicInSquadNode : Node
{

    private Squad squad;
    public MedicInSquadNode(BehaviorTree _tree) : base(_tree)
    {
        squad = behaviorTree.GetComponent<Enemy>().squad;
    }
    public override NodeStates evaluate()
    {
        if (squad == null)
        {
            state = NodeStates.Failure;
            return state;
        }
        if(!squad.hasMedics())
        {
            state = NodeStates.Failure;
            return state;
        }
        state = NodeStates.Success;
        return state;
    }


}
