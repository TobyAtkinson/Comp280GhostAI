using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceBehaviour : Node
{
    protected List<Node> nodes = new List<Node>();

    public SequenceBehaviour(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    // Sequence for ghost behaviour tree

    public override NodeState Evaluate()
    {
        bool isAnyNodeRunning = false;
        foreach(var node in nodes)
        {
            switch(node.Evaluate())
            {
                case NodeState.RUNNING:
                    isAnyNodeRunning = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    _nodeState = NodeState.FAILURE;
                    return _nodeState;

                default:
                    break;
                
                        
            }
        }
        _nodeState = isAnyNodeRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return _nodeState;
    }

}
