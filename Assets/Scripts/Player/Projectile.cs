using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType = ProjectileType.Player;
    [SerializeField] private float gravityScale = 0.0f;
    [SerializeField, Range(0, 20)] private float lifetime = 1.0f;

    public AudioClip deathSound;
    private AudioSource AudioSource;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = gravityScale != 0.0f;
        if (rb.useGravity)
            rb.mass = gravityScale; // Optionally use mass to simulate gravity scale

        Destroy(gameObject, lifetime);
    }

    public void SetVelocity(Vector3 velocity) => GetComponent<Rigidbody>().linearVelocity = velocity;

    private void OnCollisionEnter(Collision collision)
    {
        if (projectileType == ProjectileType.Player && collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die(); // Play death animation and destroy after
                Destroy(gameObject); // Destroy the projectile
            }
        }

        if (projectileType == ProjectileType.Enemy && collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && player.IsDefending)
            {
                Destroy(gameObject);
                return;
            }


            GameManager.Instance.lives--;

            Debug.Log("Player hit! Lives left: " + GameManager.Instance.lives);

            if (player != null)
            {
                player.OnDeath(); // Play death animation
                StartCoroutine(ReloadSceneAfterDelay(5f));
            }
            else
            {
                GameManager.Instance.ReloadCurrentScene();
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.ReloadCurrentScene();
    }

    public enum ProjectileType
    {
        Player,
        Enemy
    }
}
