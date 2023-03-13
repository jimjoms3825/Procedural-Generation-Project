using System.Collections.Generic;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: SelectorNode
 * Description: Main structural node of the behaviour tree. Evaluates
 * children nodes in order until one is found to be a success.
 */

public class SelectorNode : Node
{
    public SelectorNode(BehaviorTree _tree) : base(_tree) {}
    public SelectorNode(BehaviorTree _tree, List<Node> _children) : base(_tree, _children) { }

    public override NodeStates evaluate()
    {
        foreach(Node child in childNodes)
        {
            switch (child.evaluate())
            {
                case NodeStates.Failure:
                    continue;
                case NodeStates.Success:
                    state = NodeStates.Success;
                    return state;
                case NodeStates.Running:
                    state = NodeStates.Running;
                    return state;
                default:
                    continue;
            }
        }
        //No more running, all failure.
        state = NodeStates.Failure;
        return state;
    }

}
