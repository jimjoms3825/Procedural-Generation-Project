using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: SequenceNode
 * Description: One of the main structural nodes of the behaviour tree. 
 * Examines the nodes in sequence. If any node in the sequence fails, this
 * node will return a failure. 
 */
public class SequenceNode : Node
{
    public SequenceNode(BehaviorTree _tree) : base(_tree) { }
    public SequenceNode(BehaviorTree _tree, List<Node> _children) : base(_tree, _children) { }


    public override NodeStates evaluate()
    {
        bool childRunning = false;

        foreach(Node child in childNodes)
        {
            switch (child.evaluate())
            {
                case NodeStates.Failure:
                    state = NodeStates.Failure;
                    return state;
                case NodeStates.Success:
                    continue;
                case NodeStates.Running:
                    childRunning = true;
                    state = NodeStates.Running;
                    return state;
                default:
                    state = NodeStates.Success;
                    return state;
            }
        }

        if (childRunning)
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
