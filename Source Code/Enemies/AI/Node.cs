using System.Collections.Generic;

/*
 * Based on code from the following site: 
 * https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e
 */

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: Node
 * Description: This is the main structural class from which all behavior
 * tree nodes derive. This class is built for recursive execution from a 
 * root node (stored in BT).
 */

public abstract class Node
{
    public enum NodeStates { Running, Success, Failure };
    protected NodeStates state;
    
    public Node parentNode;
    public List<Node> childNodes;
    public BehaviorTree behaviorTree;

    /*
     * Constructor which takes a BehaviourTree as an argument. 
     */
    public Node(BehaviorTree _tree)
    {
        parentNode = null;
        childNodes = new List<Node>();
        behaviorTree = _tree;
    }

    /*
     * Constructor which takes a BehaviourTree and a list of nodes as
     * arguments. The constructor adds each node passed in the list 
     * as a child of this node. 
     */
    public Node(BehaviorTree _tree, List<Node> _children)
    {
        parentNode = null;
        childNodes = new List<Node>();
        foreach (Node child in _children)
        {
            attach(child);
        }
        behaviorTree = _tree;
    }

    /*
     * Convenience method which attaches the passed node as a child
     * of this node. 
     */
    public void attach(Node other)
    {
        other.parentNode = this;
        childNodes.Add(other);
    }

    /*
     * Inhereted by derived classes. This is the functional portion
     * of the class, which will be executed every frame (If decided by the
     * behaviour tree). This method returns the Node State as evaluated
     * by this node).
     */
    public abstract NodeStates evaluate();

}
