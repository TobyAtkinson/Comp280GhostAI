using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    // Setting up the ChaseNode responsible for makeing the ghost chase down and try to kill the player
    // The RangeNode checks to see if the ghost can see the player. Upon success this Node is ran
    private Blackboard blackboard;
    private NavMeshAgent agent;
    private float chaseSpeed;

    public ChaseNode(Blackboard blackboard, NavMeshAgent agent, float chaseSpeed)
    {
        this.blackboard = blackboard;
        this.agent = agent;
        this.chaseSpeed = chaseSpeed;
    }

    public override NodeState Evaluate()
    {
        // Works out the distance between itself and the player
        float distance = Vector3.Distance(blackboard.playerTransform.position, agent.transform.position);
        // If its not right ontop of the player continue to move towards them, once they collide the player automatically dies
        if (distance > 0.1f)
        {
            agent.speed = chaseSpeed;
            agent.isStopped = false;
            agent.SetDestination(blackboard.playerTransform.position);
            return NodeState.RUNNING;
        }
        // Else they are right ontop of the player which means stop.
        else
        {
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }
}
