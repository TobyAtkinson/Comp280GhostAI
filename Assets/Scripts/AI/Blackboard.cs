using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    // Blackboard used to store variables for ghosts
    // Player transform so ghosts know where to chase
    public Transform playerTransform;
    // List of positions that tell the ghost where they should patrol
    public Vector3 patrolPoint;
    public List<Vector3> possiblePatrolPoints = new List<Vector3>();
    
    // List of areas where the power pills are. If the ghost is set to defend the power pills
    // it will patrol to the closest power pill area and wait for the player to cut them off
    public List<Vector3> powerPillPoints = new List<Vector3>();
}
