using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    // Variables shown in editor to allow customization
    // Speed variables
    [Header("Ghost speed variables")]
    [Tooltip("How fast the ghost moves when patrolling around the map")]
    [SerializeField] private float patrollingSpeed = 2.5f;

    [Tooltip("How fast the ghost moves when chasing the player")]
    [SerializeField] private float chasingSpeed = 3f;

    [Tooltip("How fast the ghost moves when running away from the player")]
    [SerializeField] private float fleeingSpeed = 1.5f;

    [Tooltip("How fast the ghost moves when enraged when there are few pills left")]
    [SerializeField] private float enragedSpeed = 3.3f;

    // Time variables
    [Header("Ghoss frozen time variables")]
    [Tooltip("How long it takes the ghost to respawn when killed by player")]
    [SerializeField] private float respawnTime = 10f;

    [Tooltip("How long it takes the ghost to leave the middle area on game start")]
    [SerializeField] private float timeUntillReleased = 3f;

    // Aggressiveness variables
    [Header("Ghost aggressiveness variables ")]
    [Tooltip("How close player has to be for ghost to spot and start chase")]
    [SerializeField] private float spotPlayerRange = 11f;

    [Tooltip("How many pills left in the game for ghost to be enraged")]
    [SerializeField] private int pillsRemainingToEnraged = 15;

    // Should the ghost defend power pills bool
    [Header("Should this ghost prioritize defending power pills?")]
    [Tooltip("When ticked this ghost will patrol towards power pill locations to attempt to cut-off player")]
    [SerializeField] private bool defendPowerPills = false;

    // Private variables for holding information
    private Transform playertransform;
    private Blackboard blackboard;
    private NavMeshAgent agent;
    private Node topNode;
    private Vector3 respawnPoint;
    private float timeUntillRespawn;
  
    private void Awake()
    {
        // Getting the start point, player and agent for this ghost
        playertransform = GameObject.FindGameObjectWithTag("Player").transform;
        respawnPoint = this.transform.position;
        agent = this.GetComponent<NavMeshAgent>();
        // Generating a blackboard for the behaviour tree to use
        blackboard = new Blackboard();
        blackboard.playerTransform = playertransform;

        // Filling the list in the blackboard with the location of every pill on the map 
        // This is so the ghost can patrol to where each pill is located
        GameObject[] pills = GameObject.FindGameObjectsWithTag("pill");
        foreach(GameObject pill in pills)
        {
            blackboard.possiblePatrolPoints.Add(pill.transform.position);
        }
        // Doing the same for every power pill on the map
        GameObject[] powerPills = GameObject.FindGameObjectsWithTag("powerpill");
        foreach (GameObject powerPill in powerPills)
        {
            blackboard.powerPillPoints.Add(powerPill.transform.position);
        }

        // Ghost is frozen at the start untill released
        timeUntillRespawn = timeUntillReleased;
        // Selecting the first patrol point so the ghost knows where to first head
        FindNewPatrolPoint();
    }

    // Public method called by the player if they collide while powered
    public void KilledByPlayer()
    {
        // Ghost teleports back to the start and is frozen for aslong as the respawn time
        TeleportToRespawnPoint();
        timeUntillRespawn = respawnTime;
    }

    // Player is checking how many pills are left
    public bool AnyPillsLeft()
    {
        // If there are no more pills return false telling the player to end the game
        if (blackboard.possiblePatrolPoints.Count == 0)
        {
            return false;
        }
        // Else return true as there are some pills left
        else
        {
            return true;
        }
    }

    // Ghost is set back to the middle where it starts
    public void TeleportToRespawnPoint()
    {
        agent.Warp(respawnPoint);
    }

    // Public method called by the player when they collect a pill
    public void PillCollected(GameObject pill)
    {
        // Remove pill location 
        blackboard.possiblePatrolPoints.Remove(pill.transform.position);
    }

    public void FindNewPatrolPoint()
    {
        // If ghost is not set to defend power pills then find a random pill on the map and sets that as the new patrol point
        // This way it will eventually see the player around the map
        if (!defendPowerPills)
        {
            blackboard.patrolPoint = blackboard.possiblePatrolPoints[Random.Range(0, blackboard.possiblePatrolPoints.Count)];
        }
        // Else if ghost is set to defend power pills it will find the closest power pill to the player and go to defend that
        // This way defensive hunter ghost cuts off players route
        else
        {
            // Sets the first powerpill location as closest so we can compare
            Vector3 closestPowerPillToPlayer = blackboard.powerPillPoints[0];
            float closestPowerPillDistance = Vector3.Distance(closestPowerPillToPlayer, playertransform.position);
            for (int i = 1; i < blackboard.powerPillPoints.Count; i++)
            {
                // Goes through the list of every power pill location and compares them to eachother
                float distanceToPlayer = Vector3.Distance(blackboard.powerPillPoints[i], playertransform.position);
                if(distanceToPlayer < closestPowerPillDistance)
                {
                    // The closest one out of all powerpills is picked.
                    closestPowerPillToPlayer = blackboard.powerPillPoints[i];
                    closestPowerPillDistance = distanceToPlayer;
                }
            }
            // Sets the patrol point of defensive ghost to the closest power pill to the player
            // This way it waits in ambush
            blackboard.patrolPoint = closestPowerPillToPlayer;
        }
    }

    public bool GetPlayerPowered()
    {
        // Return true if the player is powered which lets the behaviour tree know to flee
        return playertransform.GetComponent<PacmanController>().invulnTimer > 0;
    }

    private void Start()
    {
        ConstructBehaviourTree();
    }

    private void ConstructBehaviourTree()
    {
        /* Constructing the full behaviour tree, 
        first it checks if the player is powered, if so it will flee from the player
        if not it checks if the player is in range, if so chase towards the player
        if it is also not in range of the player it patrols to somewhere on the map
        if there are only a few pills left it is enraged and chases the player even faster */

        // Constructing the bottom tier nodes
        ChaseNode chaseNode = new ChaseNode(blackboard, agent, chasingSpeed);
        RangeNode rangeNode = new RangeNode(spotPlayerRange, blackboard, this.transform);
        PatrolNode patrolNode = new PatrolNode(this, agent, blackboard, patrollingSpeed, enragedSpeed, pillsRemainingToEnraged);
        FleeNode fleeNode = new FleeNode(blackboard, agent, this.transform, fleeingSpeed);
        IsPlayerImortalNode IsPlayerImortalNode = new IsPlayerImortalNode(this);

        // Constructing the middle tier nodes
        SequenceBehaviour fleeSequence = new SequenceBehaviour(new List<Node> { IsPlayerImortalNode, fleeNode });
        SequenceBehaviour chaseSequence = new SequenceBehaviour(new List<Node> { rangeNode, chaseNode });

        // Constructing the top tier node
        topNode = new SelectorBehaviour(new List<Node> { fleeSequence, chaseSequence, patrolNode });
    }

    private void Update()
    {
        // If the ghost is dead the behaviour tree wont run as it just needs to stay still
        if(timeUntillRespawn > 0)
        {
            timeUntillRespawn -= Time.deltaTime;
        }
        // Else while alive it runs the behaviour tree to be a functioning AI
        else
        {
            topNode.Evaluate();
        }
    }
}
