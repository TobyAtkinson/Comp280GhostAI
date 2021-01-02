using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : Node
{
    // Setting up the RangeNode responsible for telling the ghost if it can see the player
    private float range;
    private Blackboard blackboard;
    private Transform origin;

    public RangeNode(float range, Blackboard blackboard, Transform origin)
    {
        this.range = range;
        this.blackboard = blackboard;
        this.origin = origin;
    }

    // Checks to see if the player is within the customizable Spot Player Range variable.
    // If so return success to chase the player
    // Else return failure to continue patrolling
    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(blackboard.playerTransform.position, origin.position);
        return distance <= range ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
