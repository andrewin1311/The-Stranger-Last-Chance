using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

    private Animator animator;  // ✅ NEW: Animator reference
    private AudioSource audioSource;  // 🔊 For zombie sounds
    public AudioClip growlSound;      // 🧟 Sound to play when near player
    private bool hasPlayedGrowl;   

    private void Awake()
    {
        player = GameObject.Find("ThirdPersonController")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure 'ThirdPersonController' exists in the scene.");
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // ✅ Initialize animator

        audioSource = GetComponent<AudioSource>();
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
            animator.SetBool("IsWalking", true);
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            animator.SetBool("IsWalking", true);
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Attack");
            AttackPlayer();
        }
        if (playerInSightRange && !hasPlayedGrowl)
        {
            PlayGrowlSound();
        }
        else if (!playerInSightRange)
        {
            hasPlayedGrowl = false; // Reset when player exits range
        }
    }

    private void PlayGrowlSound()
    {
        if (growlSound != null && audioSource != null)
        {
            audioSource.clip = growlSound;
            audioSource.Play();
            hasPlayedGrowl = true;
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
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
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
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0; // Lock the Y-axis so the zombie stays upright
        transform.rotation = Quaternion.LookRotation(lookDirection);

        if (!alreadyAttacked)
        {
            Debug.Log("💢 AttackPlayer() called");

            // Wait for the animation to finish before applying damage
            StartCoroutine(DealDamageAfterDelay()); // Adjust delay to match your punch animation length
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private IEnumerator DealDamageAfterDelay()
    {
        yield return new WaitForSeconds(0.6f);

        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);

        foreach (Collider playerCollider in hitPlayers)
        {
            Debug.Log("🎯 Hit something: " + playerCollider.name);

            if (playerCollider.CompareTag("Player"))
            {
                Debug.Log("👊 Player hit! Dealing delayed damage.");
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
