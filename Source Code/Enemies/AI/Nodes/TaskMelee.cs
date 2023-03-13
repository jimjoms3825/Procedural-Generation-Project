using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: TaskMelee
 * Description: Attempts to use a melee attack on a nearby player.
 */

public class TaskMelee : Node
{
    private Enemy enemy;
    public TaskMelee(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        //Avoids using melee every update. 
        if (behaviorTree.blackBoard.hasData("Last Melee Use"))
        {
            float time = (float)behaviorTree.blackBoard.getData("Last Melee Use");
            if (Time.time - time > enemy.meleeTime)
            {
                enemy.melee();
                state = NodeStates.Success;
                return state;
            }
            state = NodeStates.Running;
            return state;
        }
        else
        {
            enemy.melee();
        }
        state = NodeStates.Success;
        return state;
    }
}
