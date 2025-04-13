using UnityEngine;
using UnityEngine.AI;

public class BanditAI : MonoBehaviour
{
    // I used ChatGPT to figure out how to make the bandit patrol an area.

    public Transform[] patrolPoints;
    public float moveSpeed = 3f;
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public float timeToLosePlayer = 5f;

    private Transform player;
    private int currentPatrolIndex = 0;
    private float playerLostTimer = 0f;

    private enum State { Patrolling, Chasing, Attacking }
    private State currentState = State.Patrolling;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player is tagged as 'Player'");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if (distanceToPlayer < detectionRadius)
                    currentState = State.Chasing;
                break;

            case State.Chasing:
                ChasePlayer(distanceToPlayer);
                break;

            case State.Attacking:
                transform.LookAt(player);
                if (distanceToPlayer > attackRange)
                    currentState = State.Chasing;
                break;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        transform.LookAt(targetPoint.position);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer(float distanceToPlayer)
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        transform.LookAt(player);

        if (distanceToPlayer < attackRange)
        {
            currentState = State.Attacking;
            AttackPlayer();
        }
        else if (distanceToPlayer > detectionRadius)
        {
            playerLostTimer += Time.deltaTime;
            if (playerLostTimer > timeToLosePlayer)
            {
                playerLostTimer = 0f;
                currentState = State.Patrolling;
            }
        }
        else
        {
            playerLostTimer = 0f;
        }
    }

    void AttackPlayer()
    {
        // Placeholder for attack logic
        Debug.Log("Bandit attacks!");
    }
}
