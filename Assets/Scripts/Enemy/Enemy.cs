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

    Transform playerTransform;

    public EnemyState currentState;
    public Transform[] patrolPoints;
    public int pathIndex;
    public float distThreshold = 0.2f;

    [Header("Gizmos")]
    [SerializeField] private float redGizmoRadius = 1.0f; // Serialized field for red sphere radius
    [SerializeField] private float greenGizmoRadius = 2.0f; // Serialized field for green sphere radius

    private bool hasBeenPunched = false; // Prevents repeated triggers per attack

    // Animation trigger names
    private readonly string[] hitTriggers = { "Hit1", "Hit2", "Hit3" };

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponentInChildren<Animator>();
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

        // Check for attack within green gizmo sphere
        if (playerTransform != null)
        {
            float greenDistance = Vector3.Distance(transform.position, playerTransform.position);
            PlayerController playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (greenDistance <= greenGizmoRadius && playerController.IsAttacking && !hasBeenPunched)
                {
                    if (anim != null)
                    {
                        int randomIndex = Random.Range(0, hitTriggers.Length);
                        anim.SetTrigger(hitTriggers[randomIndex]);
                        Debug.Log($"Enemy hit by player within green gizmo sphere! Triggered: {hitTriggers[randomIndex]}");
                    }
                    hasBeenPunched = true;
                }
                else if (greenDistance > greenGizmoRadius || !playerController.IsAttacking)
                {
                    hasBeenPunched = false; // Reset for next attack
                }
            }
        }
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, greenGizmoRadius);
    }
}





