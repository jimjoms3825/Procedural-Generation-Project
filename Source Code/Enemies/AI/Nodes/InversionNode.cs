using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: InversionNode
 * Description: Decorator node which inverts the output of it's children.
 */

public class InversionNode : Node
{
    public InversionNode(BehaviorTree _tree) : base(_tree)
    {
    }
    public InversionNode(BehaviorTree _tree, Node child) : base(_tree, new List<Node> { child })
    {
    }

    public override NodeStates evaluate()
    {
        Node child = childNodes[0];
        if(child == null)
        {
            Debug.LogError("Trying to invert a node with no children.");
            state = NodeStates.Failure;
            return state;
        }
        if(childNodes.Count > 1)
        {
            Debug.LogError("Trying to call invert node with multiple children. All nodes outside of the first will be discarded from excecution.");
        }

        switch (child.evaluate())
        {
            case NodeStates.Failure:
                state = NodeStates.Success;
                break;
            case NodeStates.Success:
                state = NodeStates.Failure;
                break;
            case NodeStates.Running:
                state = NodeStates.Running;
                break;
            default:
                state = NodeStates.Failure;
                return state;
        }
        return state;
    }
}
