using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: CallMedicNode
 * Description: A behavior tree node which is used to call a medic
 * from the enemies squad if one exists.
 */
public class CallMedicNode : Node
{
    private Enemy enemy;
    private Squad squad;
    public CallMedicNode(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
        squad = enemy.squad;
    }

    public override NodeStates evaluate()
    {
        squad.callMedic(enemy);
        state = NodeStates.Success;
        return state;
    }

}
