using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeNode : Node
{
    // Setting up FleeNode, this is reponsible for making the ghost run away from the player when they are powered.
    // The IsPlayerImortalNode checks to see if the player is powered, if they are this Node is ran
    private Blackboard blackboard;
    private NavMeshAgent agent;
    private Transform origin;
    private float fleeingSpeed;

    public FleeNode(Blackboard blackboard, NavMeshAgent agent, Transform origin, float fleeingSpeed)
    {
        this.blackboard = blackboard;
        this.agent = agent;
        this.origin = origin;
        this.fleeingSpeed = fleeingSpeed;
    }

    public override NodeState Evaluate()
    {
        // Slows the ghost down so they are easier for the player to catch
        agent.speed = fleeingSpeed;

        // Works out which direction is opposite to the player
        Vector3 dirToPlayer = origin.transform.position - blackboard.playerTransform.position;
        Vector3 newPos = origin.transform.position + dirToPlayer;

        // Returns running to show it is succsessfully fleeing in the opposite direction to the player
        agent.SetDestination(newPos);
        return NodeState.RUNNING;
    }
}
