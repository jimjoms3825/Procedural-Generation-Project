using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Based on code from the following site: 
 * https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e
 */

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: BehaviourTree
 * Description: The abstract class from which all behaviour 
 * tree implementations derive. Uses recursive node evaluation once
 * every frame.
 */

public abstract class BehaviorTree : MonoBehaviour
{
    protected Node rootNode = null;
    public BlackBoard blackBoard;

    private void Awake()
    {
        blackBoard = new BlackBoard();
        rootNode = setupTree();
    }

    private void LateUpdate()
    {
        if(rootNode == null) { return; }
        //Recursive evaluation through the root node.
        rootNode.evaluate();
    }

    /*
     * Abstract method in which derived classes define their BT structure. 
     */
    protected abstract Node setupTree();
}
