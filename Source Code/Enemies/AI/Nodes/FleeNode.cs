using UnityEngine;
using UnityEngine.AI;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: FleeNode
 * Description: Flees the AI agent away from the player.
 */

public class FleeNode : Node
{
    private Enemy enemy;

    public FleeNode(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        Vector3 targetPos = behaviorTree.transform.position;
        Vector3 fleeDirection = targetPos - GameManager.player.transform.position;
        fleeDirection.Normalize();
        targetPos += fleeDirection;
        behaviorTree.blackBoard.addData("Flee Position", targetPos);
        state = NodeStates.Success;
        enemy.playVoiceLine(Enemy.VoiceLines.Moving);
        return state;
    }

}
