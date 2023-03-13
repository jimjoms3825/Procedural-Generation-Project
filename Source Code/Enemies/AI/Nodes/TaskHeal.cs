/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: TaskHeal
 * Description: Medic behaviour tree node which heals an enemy. The enemy
 * must be navigated to before this node is excecuted, as this node does
 * not check for proximity.
 */
public class TaskHeal : Node
{
    private Enemy enemy;
    public TaskHeal(BehaviorTree _tree) : base(_tree)
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        Enemy healEnemy = (Enemy)behaviorTree.blackBoard.getData("Heal Enemy");
        enemy.heal(healEnemy);
        state = NodeStates.Success;
        return state;
    }
}
