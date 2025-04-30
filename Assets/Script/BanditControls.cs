using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BanditControl : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    private Animator animator;
    private Coroutine patrolRoutine;

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
        animator = GetComponent<Animator>();
        patrolRoutine = StartCoroutine(Patroling()); // Keep the reference
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
            if (patrolRoutine == null)
            {
                Debug.Log("🧍 Player not seen. Resuming patrol...");
                patrolRoutine = StartCoroutine(Patroling());
            }
        }
        else
        {
            if (patrolRoutine != null)
            {
                Debug.Log("❌ Stopping patrol to engage player.");
                StopCoroutine(patrolRoutine);
                patrolRoutine = null;
                agent.isStopped = false; // Resume navmesh movement
            }

            if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();
            }
            else if (playerInAttackRange && playerInSightRange)
            {
                AttackPlayer();
            }
        }
    }

    private void ForgetPlayer()
    {
        playerWasSeenRecently = false;
        walkPointSet = false;
        Debug.Log("Bandit forgot the player. Returning to patrol.");
    }

    private IEnumerator Patroling()
    {
        agent.speed = 1f; // Normal walking speed
        animator.SetBool("IsSprinting", false); // Stop sprint animation

        while (true) // Keep patrolling forever
        {
            // Idle
            animator.SetBool("IsWalking", false);
            animator.Play("IdleR_To_IdleL_1");
            agent.isStopped = true;

            yield return new WaitForSeconds(5f);

            // Walk
            SearchWalkPoint();
            if (walkPointSet)
            {
                agent.SetDestination(walkPoint);
                agent.isStopped = false;
                animator.SetBool("IsWalking", true);
                animator.Play("WalkFWD");

                // Wait until destination is reached
                while (Vector3.Distance(transform.position, walkPoint) > 1f)
                {
                    yield return null;
                }
            }
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

        Debug.LogWarning("⚠ Could not find valid patrol point.");
    }

    private void ChasePlayer()
    {
        agent.speed = 2f; // Sprint speed (adjust to your needs)
        agent.SetDestination(player.position);

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsSprinting", true); // Optional for sprint animation
        animator.Play("SprintJump_ToLeft_R");
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0; // Lock the Y-axis so the bandit stays upright
        transform.rotation = Quaternion.LookRotation(lookDirection);

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
                        Debug.LogWarning("⚠ PlayerHealth script not found on Player!");
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
