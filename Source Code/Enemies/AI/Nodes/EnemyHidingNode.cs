using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: EnemyHidingNode
 * Description: Checks to see whether the player is out of the 
 * enemies vision in combat, or "hiding". 
 */

public class EnemyHidingNode : Node
{
    private BlackBoard squadBlackBoard;
    private float timeToAction;

    public EnemyHidingNode(BehaviorTree _tree, float _timeToAction) : base(_tree)
    {
        if (behaviorTree.GetComponent<Enemy>().squad != null)
        {
            squadBlackBoard = behaviorTree.GetComponent<Enemy>().squad.blackBoard;
        }
        timeToAction = _timeToAction;
    }

    public override NodeStates evaluate()
    {
        if(squadBlackBoard == null)
        {
            if (behaviorTree.GetComponent<Enemy>().squad != null)
            {
                squadBlackBoard = behaviorTree.GetComponent<Enemy>().squad.blackBoard;
            }
        }
        else if(squadBlackBoard.hasData("Player Last Seen"))
        {
            float playerLastSeenTime = (float)squadBlackBoard.getData("Player Last Seen");
            if (Time.time - playerLastSeenTime > timeToAction)
            {
                state = NodeStates.Success;
                return state;
            }
        }
        state = NodeStates.Failure;
        return state;
    }

}
