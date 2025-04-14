using UnityEngine;
using UnityEngine.AI;

public class ZombieMover : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 100f;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 10f;

    // Attacking
    public float timeBetweenAttacks = 1.5f;
    bool alreadyAttacked;

    // States
    public float sightRange = 15f, attackRange = 2f;
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
            Debug.Log("🧍 Player not seen. Trying to patrol...");
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
        Debug.Log("🧠 Zombie forgot the player. Returning to patrol.");
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            Debug.Log("🔍 Searching for new patrol point...");
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            Debug.Log("🚶 Moving to walk point: " + walkPoint);
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            Debug.Log("✅ Reached patrol point.");
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
                Debug.Log("🟢 Found patrol point on NavMesh: " + walkPoint);
                return;
            }
        }

        Debug.LogWarning("⚠️ Could not find valid patrol point.");
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Debug.Log("💢 AttackPlayer() called");

            Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);

            foreach (Collider playerCollider in hitPlayers)
            {
                Debug.Log("🎯 Hit something: " + playerCollider.name);

                if (playerCollider.CompareTag("Player"))
                {
                    Debug.Log("👊 Player hit! Dealing damage.");
                    PlayerHealth ph = playerCollider.GetComponent<PlayerHealth>();
                    if (ph != null)
                    {
                        ph.TakeDamage(10);
                    }
                    else
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
