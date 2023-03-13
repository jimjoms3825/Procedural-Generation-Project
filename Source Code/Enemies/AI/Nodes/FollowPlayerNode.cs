
/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: FollowPlayerNode
 * Description: Stores the position of the player so the enemy can move in
 * their direction.
 */

public class FollowPlayerNode : Node
{
    private BlackBoard squadBlackBoard;
    public FollowPlayerNode(BehaviorTree _tree) : base(_tree)
    {
        if(behaviorTree.GetComponent<Enemy>().squad != null)
        {
            squadBlackBoard = behaviorTree.GetComponent<Enemy>().squad.blackBoard;
        }
    }

    public override NodeStates evaluate()
    {
        if (squadBlackBoard == null)
        {
            if (behaviorTree.GetComponent<Enemy>().squad != null)
            {
                squadBlackBoard = behaviorTree.GetComponent<Enemy>().squad.blackBoard;
            }
        }
        else if (squadBlackBoard.hasData("Player Position"))
        {
            behaviorTree.blackBoard.addData("Player Position", squadBlackBoard.getData("Player Position"));
            state = NodeStates.Success;
            return state;
        }
        state = NodeStates.Failure;
        return state;
    }

}
