using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerImortalNode : Node
{
    // Setting up IsPlayerImortalNode, this is responsible for checking if the player is powered
    private GhostAI ai;

    public IsPlayerImortalNode(GhostAI ai)
    {
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        // If the player is powered return success which will tell the FleeNode to run away
        if(ai.GetPlayerPowered() == true)
        {
            return NodeState.SUCCESS;
        }
        // Else player is not powered which will return failure and will make the ghost go back to patrolling or chasing the player
        else
        {
            return NodeState.FAILURE;
        }
    }
}
