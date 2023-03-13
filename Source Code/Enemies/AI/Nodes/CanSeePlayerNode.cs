using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: CanSeePlayerNode
 * Description: Checks to see if the enemy can see the player.
 */

public class CanSeePlayerNode : Node
{
    public CanSeePlayerNode(BehaviorTree _tree) : base(_tree)
    {
    }

    public override NodeStates evaluate()
    {
        Vector3 playerDirection = GameManager.player.transform.position - behaviorTree.transform.position;
        playerDirection.Normalize();
        RaycastHit hit;
        //Shoot a ray from the enemy to the player.
        Physics.Raycast(behaviorTree.transform.position + Vector3.up, playerDirection, out hit, 50);
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            state = NodeStates.Success;
            return state;
        }
        state = NodeStates.Failure;
        return state;
    }

}
