using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: FacePlayerNode
 * Description: Rotates the enemy toward the player.
 */
public class FacePlayerNode : Node
{
    public FacePlayerNode(BehaviorTree _tree) : base(_tree)
    {
    }

    public override NodeStates evaluate()
    {
        Vector3 lookTarget = GameManager.player.transform.position;
        lookTarget.y = behaviorTree.transform.position.y;
        Quaternion toRotation = Quaternion.LookRotation(lookTarget - behaviorTree.transform.position);
        behaviorTree.transform.rotation = Quaternion.Lerp(behaviorTree.transform.rotation, toRotation, 5 * Time.deltaTime);
        state = NodeStates.Success;
        return state;
    }

}
