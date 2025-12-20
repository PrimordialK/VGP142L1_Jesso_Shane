using UnityEngine;

public class BooEnemy : Enemy
{
    [SerializeField] private float moveSpeed = 2f;
    private Transform player;
    private EnemyShoot enemyShoot;
    private SkinnedMeshRenderer smr;
    private float shootCooldown = 3.0f; // seconds between shots
    private float lastShootTime = -Mathf.Infinity;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    void Start()
    {// base.Start();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("Player not found in scene!");

        // Get the EnemyShoot component
        enemyShoot = GetComponent<EnemyShoot>();
        if (enemyShoot == null)
            Debug.LogWarning("EnemyShoot component not found on BooEnemy!");

        

        // Get SkinnedMeshRenderer from child
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr == null)
            Debug.LogWarning("SkinnedMeshRenderer not found in children of BooEnemy!");
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toEnemy = (transform.position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, toEnemy);
        bool playerLooking = angle <= 45f;


        if (smr != null)
            smr.enabled = !playerLooking; // Only visible when NOT being looked at

        if (!playerLooking)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(player.position);

            // Check distance and shoot if within 10 meters
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= 10f && Time.time - lastShootTime >= shootCooldown)
            {
                if (enemyShoot != null)
                {
                    enemyShoot.Fire();
                    lastShootTime = Time.time;
                }
            }
        }
    }
}
