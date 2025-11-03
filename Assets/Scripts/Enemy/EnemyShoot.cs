using UnityEngine;
using UnityEngine.ProBuilder;

public class EnemyShoot : MonoBehaviour
{
    public AudioClip shootSound;

    private AudioSource audioSource;
    [SerializeField] private float shotSpeed = 10.0f;
    [SerializeField] private Projectile projectilePrefab = null;

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

        // Spawn at the front of the object
        Vector3 spawnPos = transform.position + transform.forward;
        Projectile curProjectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // Set velocity in the forward direction
        Vector3 velocity = transform.forward * shotSpeed;
        curProjectile.SetVelocity(velocity);

        audioSource?.PlayOneShot(shootSound);
    }
}