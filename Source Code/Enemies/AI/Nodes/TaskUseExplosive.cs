using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: TaskUseExplosive
 * Description: Uses an explosive if the time condition has met. 
 */
public class TaskUseExplosive : Node
{
    private Enemy enemy;
    public TaskUseExplosive(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        if(behaviorTree.blackBoard.hasData("Last Explosive Use"))
        {
            float time = (float)behaviorTree.blackBoard.getData("Last Explosive Use");
            if(Time.time - time > enemy.explosiveTime)
            {
                enemy.useExplosive();
                state = NodeStates.Success;
                return state;
            }
            state = NodeStates.Running;
            return state;
        }
        else
        {
            enemy.useExplosive();
        }
        state = NodeStates.Success;
        return state;
    }
}
