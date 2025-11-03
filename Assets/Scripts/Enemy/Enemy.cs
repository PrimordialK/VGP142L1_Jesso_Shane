using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public AudioClip deathSound;
    private AudioSource audioSource;
    protected Collider col;
    protected SpriteRenderer sr;
    protected Animator anim;
    protected int health;

    [SerializeField] private int maxHealth = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {

        if (deathSound != null)
        {

            TryGetComponent(out audioSource);

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("AudioSource component was missing. Added one dynamically.");
            }
        }
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (maxHealth <= 0)
        {
            Debug.Log("maxHealth must be greater than 0. Setting to 5.");
            maxHealth = 5;
        }
        health = maxHealth;
    }

    // Update is called once per frame
    public virtual void TakeDamage(int damageValue, DamageType damagetype = DamageType.Default)
    {
        health -= damageValue;

        if (health <= 0)
        {
            anim.SetTrigger("Death");

            if (transform.parent != null)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                Destroy(transform.parent.gameObject, 5.0f);
            }
            else
            {
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                Destroy(gameObject, 5.0f);
            }
            audioSource?.PlayOneShot(deathSound);
        }
    }
}

public enum DamageType
{
    Default,
    JumpedOn
}

