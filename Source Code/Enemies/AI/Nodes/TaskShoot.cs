using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: TaskShoot
 * Description: Calls the enemy.shoot() method if shoot timer condition
 * is met. 
 */
public class TaskShoot : Node
{
    private Enemy enemy;
    public TaskShoot(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        //Avoids shooting faster than intended. 
        if (behaviorTree.blackBoard.hasData("Last Shot"))
        {
            float time = (float)behaviorTree.blackBoard.getData("Last Shot");
            if (Time.time - time > enemy.shootTime)
            {
                enemy.shoot();
                state = NodeStates.Success;
                return state;
            }
            state = NodeStates.Running;
            return state;
        }
        else
        {
            enemy.shoot();
        }
        state = NodeStates.Success;
        return state;
    }

}
