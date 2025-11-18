using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Chase, Patrol
    }    

    //components
    NavMeshAgent agent;
    Transform target;
    Animator anim;
    EnemyShoot shootScript; // Reference to the shoot script

    Transform playerTransform;

    public EnemyState currentState;
    public Transform[] patrolPoints;
    public int pathIndex;
    public float distThreshold = 0.2f;

    [Header("Gizmos")]
    [SerializeField] private float redGizmoRadius = 1.0f; // Serialized field for red sphere radius

    [Header("Drop Settings")]
    [SerializeField] private GameObject itemDropPrefab; // Assign your item prefab in the Inspector

    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponentInChildren<Animator>();
        shootScript = GetComponent<EnemyShoot>(); // Get the shoot script attached to this enemy
    }

    void Update()
    {
        bool playerNearby = false;
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            playerNearby = distance <= redGizmoRadius;
        }

        anim.SetBool("PlayerNearby", playerNearby);

        if (playerNearby)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            // Shoot at the player when within the red gizmo radius
            if (shootScript != null)
            {
                shootScript.Fire();
            }
        }
        else
        {
            agent.isStopped = false;

            if (currentState == EnemyState.Chase)
            {
                ChasePlayer();
            }
            else if (currentState == EnemyState.Patrol)
            {
                Patrol();
            }

            if (!target) throw new System.Exception("Enemy has no target set!");
            agent.SetDestination(target.position);
        }

        // Update speed parameter for blend tree
        anim.SetFloat("speed", agent.velocity.magnitude);
    }

    void ChasePlayer()
    {
        if (!playerTransform) return;
        target = playerTransform;
        Debug.Log($"ChasePlayer called. Target position: {target.position}");
    }

    void Patrol()
    {
        if (target == playerTransform) target = patrolPoints[pathIndex];
        if (agent.remainingDistance < distThreshold)
        {
            pathIndex++;
            pathIndex %= patrolPoints.Length;
            target = patrolPoints[pathIndex];
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, redGizmoRadius);
    }

    public void OnShoot()
    {
        if (shootScript != null)
        {
            shootScript.Fire();
            Debug.Log("Animation event: Enemy fired projectile.");
        }
    }

    public void Die()
    {
        if (isDead) return; // Prevent multiple deaths
        isDead = true;

        if (anim != null)
            anim.SetTrigger("Death"); // Play death animation

        // Drop item at enemy's position
        if (itemDropPrefab != null)
        {
            Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 2.0f); // Destroy after animation (adjust delay as needed)
    }
}








