using UnityEngine;
using UnityEngine.AI;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: TaskFlankPlayer
 * Description: Uses the player position to flank a player.
 */

public class TaskFlankPlayer : Node
{
    private Vector3 playerPos;
    private BlackBoard squadBlackBoard;
    private NavMeshAgent navMeshAgent;
    private Enemy enemy;
    public TaskFlankPlayer(BehaviorTree _tree) : base(_tree)
    {
        navMeshAgent = behaviorTree.GetComponent<NavMeshAgent>();
        enemy = behaviorTree.GetComponent<Enemy>();
        if (enemy.squad != null)
        {
            squadBlackBoard = enemy.squad.blackBoard;
        }
    }

    public override NodeStates evaluate()
    {
        if(squadBlackBoard == null)
        {
            if (enemy.squad != null)
            {
                squadBlackBoard = enemy.squad.blackBoard;
            }
        }
        else
        {
            enemy.playVoiceLine(Enemy.VoiceLines.Flanking);
            playerPos = (Vector3)squadBlackBoard.getData("Player Position");
            if (navMeshAgent != null && playerPos != null)
            {
                navMeshAgent.destination = playerPos;

                if (Vector3.Distance(navMeshAgent.transform.position, navMeshAgent.destination) < 0.5f)
                {
                    state = NodeStates.Success;
                    return state;
                }
            }
            state = NodeStates.Running;
        }
        return state;
    }

}
