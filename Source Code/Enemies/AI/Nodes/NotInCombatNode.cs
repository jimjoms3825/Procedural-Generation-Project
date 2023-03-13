/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: NotInCombatNode
 * Description: Alerts the behaviour tree if the agent's squad has
 * entered combat. 
 */
public class NotInCombatNode : Node
{
    private bool inCombat = false;
    private Enemy enemy;

    public NotInCombatNode(BehaviorTree _tree) : base(_tree) 
    {
        enemy = behaviorTree.GetComponent<Enemy>();
    }

    public override NodeStates evaluate()
    {
        if (enemy.squad == null || !enemy.squad.blackBoard.hasData("In Combat"))
        {
            state = NodeStates.Success;
            return state;
        }
        if(inCombat == false)
        {
            enemy.playVoiceLine(Enemy.VoiceLines.Engaging);
        }
        inCombat = (bool)enemy.squad.blackBoard.getData("In Combat");
       
        if (inCombat)
        {
            state = NodeStates.Failure;
            return state;
        }
        state = NodeStates.Success;
        return state;
    }
}

