using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: PlayerInRangeNode
 * Description: Lets the behaviour tree know if the player is within the 
 * range specified in the constructor. 
 */
public class PlayerInRangeNode : Node
{
    private float range;
    private GameObject player;
    public PlayerInRangeNode(BehaviorTree _tree, float _range) : base(_tree)
    {
        range = _range;
        player = GameManager.player;
    }

    public override NodeStates evaluate()
    {
        if(player == null || 
            Vector3.Distance(behaviorTree.transform.position, player.transform.position) > range){
            state = NodeStates.Failure;
        }
        else
        {
            state = NodeStates.Success;
        }
        return state;
    }

}
