using UnityEngine;
using UnityEngine.AI;

public class BanditAI : MonoBehaviour
{
    // Code used from Tom. 

    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 100f;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 2.5f;

    // Attacking
    public float timeBetweenAttacks = 1.5f;
    bool alreadyAttacked;

    // States
    public float sightRange = 2f, attackRange = 0f;
    public bool playerInSightRange, playerInAttackRange;
    private bool playerWasSeenRecently;

    private void Awake()
    {
        player = GameObject.Find("ThirdPersonController")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure 'ThirdPersonController' exists in the scene.");
        }

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange)
        {
            playerWasSeenRecently = true;
            CancelInvoke(nameof(ForgetPlayer));
        }
        else if (!IsInvoking(nameof(ForgetPlayer)))
        {
            Invoke(nameof(ForgetPlayer), 3f);
        }

        if (!playerWasSeenRecently)
        {
            Patroling();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
    }

    private void ForgetPlayer()
    {
        playerWasSeenRecently = false;
        walkPointSet = false;
        Debug.Log("🧠 Bandit forgot the player. Returning to patrol.");
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            Vector3 targetWalkPoint = walkPoint;
            targetWalkPoint.y = transform.position.y; // Keep bandit on same Y level for patrol
            agent.SetDestination(targetWalkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        for (int i = 0; i < 30; i++)
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            Vector3 potentialPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (NavMesh.SamplePosition(potentialPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                walkPoint = hit.position;
                walkPointSet = true;
                return;
            }
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        Debug.Log("📏 Chasing Player - Distance: " + Vector3.Distance(transform.position, player.position));
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position); // Stop moving while attacking
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Debug.Log("💢 AttackPlayer() called");

            Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);

            foreach (Collider playerCollider in hitPlayers)
            {
                if (playerCollider.CompareTag("Player"))
                {
                    Debug.Log("👊 Player hit! Dealing damage.");
                    // Uncomment when you have a health script
                    // PlayerHealth ph = playerCollider.GetComponent<PlayerHealth>();
                    // if (ph != null)
                    // {
                    //     ph.TakeDamage(10);
                    // }
                    // else
                    {
                        Debug.LogWarning("⚠️ PlayerHealth script not found on Player!");
                    }
                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}