using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: WanderNode
 * Description: Wanders the AI agent randomly by setting it's target position
 * at regular intervals. 
 */
public class WanderNode : Node
{
    private float changeTimer;
    private float secondsToChange;
    private Enemy enemy;

    public WanderNode(BehaviorTree _tree, float maxSecondsToChange) : base(_tree) {
        secondsToChange = Random.Range(maxSecondsToChange / 2, maxSecondsToChange);
        changeTimer = secondsToChange;
        enemy = behaviorTree.GetComponent<Enemy>();
    }


    //Tells the parent node a random position to move toward. 
    public override NodeStates evaluate()
    {
        changeTimer += Time.deltaTime;
        if(changeTimer < secondsToChange)
        {
            state = NodeStates.Success;
            return state;
        }
        //Reset change timer
        changeTimer = 0;
        //Grab a random position up to 20 meters away in any direction.
        Vector3 randomPosition = behaviorTree.transform.position;
        randomPosition += new Vector3(Random.Range(20, -20), Random.Range(20, -20), Random.Range(20, -20));
        behaviorTree.blackBoard.addData("Wander Position", randomPosition);

        if(Random.Range(0, 1f) > 0.5f)
        {
            enemy.playVoiceLine(Enemy.VoiceLines.Moving);
        }

        state = NodeStates.Success;
        return state;
    }

}
