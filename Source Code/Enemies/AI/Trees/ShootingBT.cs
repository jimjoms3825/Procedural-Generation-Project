using System.Collections.Generic;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: ShootingBT
 * Description: A behaviour tree with similar function to the CombatBT, 
 * however the ability to melee and use explosives is removed. 
 */

public class ShootingBT : BehaviorTree
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
            
            //Second main branch
            new SelectorNode(this, new List<Node>
            {
                //Medic Branch
                new SequenceNode(this, new List<Node>
                {
                    new LowHealthNode(this, 0.5f),
                    new MedicInSquadNode(this),
                    new SequenceNode(this, new List<Node>{
                        new CallMedicNode(this),
                        new WaitForMedicNode(this, 0.5f)
                    })
                }),

                //Melee and flee branch
                new SequenceNode(this, new List<Node>
                {
                    new PlayerInRangeNode(this, 5), // flee distance
                    new FleeNode(this),
                    new FacePlayerNode(this),
                    new TaskGoToTarget("Flee Position", this)
                }),

                //Shooting branch
                
                new SequenceNode(this, new List<Node>
                {
                    new PlayerInRangeNode(this, 30), // Shooting range
                    new SelectorNode(this, new List<Node>
                    {
                        //Cover branch
                        new SequenceNode(this, new List<Node>
                        {
                            new EnemyHidingNode(this, 2f),
                            //Only Flank
                            new TaskFlankPlayer(this)
                        }),

                        //Shooting branch
                        new SequenceNode(this, new List<Node>
                        {
                            //aim
                            new CanSeePlayerNode(this),
                            new TaskStopMoving(this),
                            new FacePlayerNode(this),
                            //fire
                            new TaskShoot(this)
                        })
                    })
                }),
                //Move to shooting range
                new SequenceNode(this, new List<Node>
                {
                    new InversionNode(this, new PlayerInRangeNode(this, 30)),
                    new FollowPlayerNode(this),
                    new TaskGoToPositionUpdate("Player Position", this)
                })
            })
        });
        return rootNode;
    }
}
