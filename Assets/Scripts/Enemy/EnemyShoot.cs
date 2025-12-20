using UnityEngine;


public class EnemyShoot : MonoBehaviour
{
    public AudioClip shootSound;

    private AudioSource audioSource;
    [SerializeField] private float shotSpeed = 10.0f;
    [SerializeField] private Projectile projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;

    void Start()
    {
        if (shootSound != null)
        {
            TryGetComponent(out audioSource);

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("AudioSource component was missing. Added one dynamically.");
            }
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not set. Please assign it in the inspector.");
        }
    }

    public void Fire()
    {
        if (projectilePrefab == null) return;

        // Spawn at the specified projectile spawn point or at the front of the object
        Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position + transform.forward;
        Quaternion spawnRot = projectileSpawnPoint != null ? projectileSpawnPoint.rotation : Quaternion.identity;
        Projectile curProjectile = Instantiate(projectilePrefab, spawnPos, spawnRot);

        // Set velocity in the forward direction
        Vector3 velocity = transform.forward * shotSpeed;
        curProjectile.SetVelocity(velocity);

        audioSource?.PlayOneShot(shootSound);
    }

    public void Fire(GameObject projectilePrefab, Vector3 position, Quaternion rotation)
    {
        if (projectilePrefab != null)
        {
            Instantiate(projectilePrefab, position, rotation);
            // Optionally: play sound, set velocity, etc.
            if (audioSource != null && shootSound != null)
                audioSource.PlayOneShot(shootSound);
        }
    }
}