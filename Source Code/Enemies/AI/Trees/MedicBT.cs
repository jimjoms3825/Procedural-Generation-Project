using System.Collections.Generic;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: MedicBT
 * Description: A behaviour tree which is suitable for Enemies with healing
 * capabilities. This BT gives the agent the ability to heal squadmates, shoot
 * flee, etc.
 */
public class MedicBT : BehaviorTree
{
    protected override Node setupTree()
    {
        rootNode = new SelectorNode(this, new List<Node>
        {
            //First Branch
            new SequenceNode(this, new List<Node>
            {
                new NotInCombatNode(this),
                new WanderNode(this, 10),
                new TaskGoToTarget("Wander Position", this)
            }),

            //Second Branch
            new SelectorNode(this, new List<Node>
            {
                //Heal Branch
                new SequenceNode(this, new List<Node>
                {
                    new HasMedicRequestsNode(this),
                    new GetHealPositionNode(this),
                    new TaskGoToTarget("Heal Position", this),
                    new TaskHeal(this)
                }),
                //Flee branch
                new SequenceNode(this, new List<Node>
                {
                    new PlayerInRangeNode(this, 10),
                    new FleeNode(this),
                    new TaskGoToPositionUpdate("Flee Position", this)
                }),
                //Fire branch
                new SelectorNode(this, new List<Node>
                {
                    new SequenceNode(this, new List<Node>
                    {
                        new PlayerInRangeNode(this, 30), // short range
                        new FacePlayerNode(this),
                        new TaskShoot(this)
                    }),
                    new SequenceNode(this, new List<Node>
                    {
                        new FollowPlayerNode(this),
                        new TaskGoToPositionUpdate("Player Position", this)
                    })
                })
            })
        });
        return rootNode;
    }
}
