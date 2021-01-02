using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolNode : Node
{
    // Setting up PatrolNode responsible for making ghost move to random chosen pill location on map
    // This runs if the ghost is not fleeing or chasing the player
    private GhostAI ai;
    private NavMeshAgent agent;
    private Blackboard blackboard;
    private float patrollingSpeed;
    private float chaseSpeed;
    private float pillsRemainingToEnraged;


    public PatrolNode(GhostAI ai, NavMeshAgent agent, Blackboard blackboard, float patrollingSpeed, float chaseSpeed, int pillsRemainingToEnraged)
    {
        this.ai = ai;
        this.agent = agent;
        this.blackboard = blackboard;
        this.patrollingSpeed = patrollingSpeed;
        this.chaseSpeed = chaseSpeed;
        this.pillsRemainingToEnraged = pillsRemainingToEnraged;
    }

    public override NodeState Evaluate()
    {
        if (blackboard.possiblePatrolPoints.Count < pillsRemainingToEnraged)
        {
            // If there are less than the select pills remaining to enraged left the ghosts become enraged and will quickly hunt down player instead of patrolling
            // This to add extra pressure for when the player has nearly won
            agent.speed = chaseSpeed;
            agent.isStopped = false;
            agent.SetDestination(blackboard.playerTransform.position);
            // Return running to show ghost is now enraged and chasing player
            return NodeState.RUNNING;
        }
        else
        {
            // Else, ghosts will pick a random pill left on it and patrol towards it
            agent.speed = patrollingSpeed;
            float distance = Vector3.Distance(blackboard.patrolPoint, agent.transform.position);
            if (distance > 2f)
            {
                // Carry on patrolling
                agent.isStopped = false;
                agent.SetDestination(blackboard.patrolPoint);
                // Return running to show ghost is patrolling towards chosen point
                return NodeState.RUNNING;
            }
            else
            {
                // Once ghost has touched chosen patrol point return success and pick new point
                agent.isStopped = true;
                ai.FindNewPatrolPoint();
                return NodeState.SUCCESS;
            }
        } 
    }
}
