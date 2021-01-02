using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Node
{
    protected NodeState _nodeState;

    public NodeState nodeState { get { return _nodeState; } }

    public abstract NodeState Evaluate();
}

// Setting up the enums to be used in the behaviour tree
public enum NodeState
{
    RUNNING, SUCCESS, FAILURE,
}
