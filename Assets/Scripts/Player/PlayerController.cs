using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    private Camera mainCamera;
    private Animator anim;

    [SerializeField] public Transform weaponAttachPoint1; // Equipped weapon (e.g., hand)
    [SerializeField] public Transform weaponAttachPoint2; // Holstered weapon (e.g., back/hip)


    public int health;
    public float cash;
    public bool collectWeapon;

    private WeaponBase equippedWeapon = null;
    private WeaponBase holsteredWeapon = null;

    private float curSpeed = 2.0f;
    [Header("Movement Settings")]
    [SerializeField] private float initSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 15.0f;
    [SerializeField] private float moveAccel = 2f;
    [SerializeField] private float rotationSpeed = 5.0f;

    [Header("Jump Settings")]
    [SerializeField] public float jumpHeight = 2.0f;
    [SerializeField] private float timeToJumpApex = 0.4f;

    public LayerMask weaponLayerMask;
    public LayerMask enemyLayerMask;
    //Movement variables
    float gravity;
    float initialJumpVelocity;

    Vector2 direction; //direction of movement - no gravity is applied here
    Vector3 velocity;

    bool jumpPressed = false;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip respawnSound;
    private AudioSource audioSource;

    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            cc = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();
            if (cc == null) throw new UnassignedReferenceException("CharacterController component is not assigned!");
        }
        catch (UnassignedReferenceException e)
        {
            //do something here
            Application.Quit();
        }
        finally
        {
            //this code always runs after the try-catch block no matter if an exeption was thrown or not
        }

        mainCamera = Camera.main;
        CalculateJumpVariables();

        //Layer 6 is Weapons
        //LayerMask.GetMask("Weapon") seems to be inconsistent (in testing it was pulling layer 64) - setting it directly works better
        weaponLayerMask = 6;
        enemyLayerMask = 3;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        InputManager.Instance.OnMoveEvent += OnMove;
        InputManager.Instance.OnJumpEvent += OnJump;
        InputManager.Instance.OnDropEvent += OnDrop;
        InputManager.Instance.OnAttackEvent += OnAttack;
        InputManager.Instance.OnDefendEvent += OnDefend;
        InputManager.Instance.OnDeathEvent += OnDeath;
        InputManager.Instance.OnSwitchWeaponsEvent += OnSwitchWeapons;
    }

    void OnValidate()
    {
        CalculateJumpVariables();
    }

    public void OnJump(bool pressed) => jumpPressed = pressed;
    public void OnMove(Vector2 movementDir) => direction = movementDir;

    public void OnDrop(bool dropEquipped)
    {
        if (dropEquipped && equippedWeapon != null)
        {
            equippedWeapon.Drop(GetComponent<Collider>());
            equippedWeapon = null;
        }
        else if (!dropEquipped && holsteredWeapon != null)
        {
            holsteredWeapon.Drop(GetComponent<Collider>());
            holsteredWeapon = null;
        }
    }

    public bool IsAttacking { get; private set; }

    public void OnAttack()
    {
        if (anim != null)
            anim.SetTrigger("Attack1"); // This triggers the attack animation

        //if (equippedWeapon != null)
        //    equippedWeapon.Shoot(); // If you want to keep this, but avoid double firing
    }

    public bool IsDefending { get; private set; }

    public void OnDefend(bool isDefending)
    {
        Debug.Log("Defend input: " + isDefending);
        if (anim != null)
            anim.SetBool("IsDefending", isDefending);
        IsDefending = isDefending;
    }

    public void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null)
            anim.SetTrigger("Death");

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Optionally, wait for the death animation to finish before destroying
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    private System.Collections.IEnumerator DestroyAfterDeathAnimation()
    {
        float deathAnimLength = 1.0f;
        if (anim != null)
        {
            foreach (var clip in anim.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "Death")
                {
                    deathAnimLength = clip.length;
                    break;
                }
            }
        }
        yield return new WaitForSeconds(deathAnimLength);
        Destroy(gameObject); 
      
    }

    // Update is called once per frame
    void Update()
    {
        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(newRay.origin, newRay.direction * 10f, Color.red, 0.1f);

        if (Physics.Raycast(newRay, out hitInfo, 10.0f, LayerMask.GetMask("Enemy")))
        {
            Debug.Log("Enemy in front of player: " + hitInfo.collider.name);
        }

        // Reset attack state after processing (or after a short time)
        IsAttacking = false;
    }

    void FixedUpdate()
    {
        //apply movement
        Vector3 projectedMoveDir = ProjectedMoveDirection();
        UpdateCharacterVelocity(projectedMoveDir);

        cc.Move(velocity * Time.fixedDeltaTime);
        if (anim != null)
            anim.SetFloat("speed", curSpeed / maxSpeed);

        //apply rotation
        if (direction != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(projectedMoveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    #region MovementCalculations
    private void UpdateCharacterVelocity(Vector3 projectedMoveDir)
    {
        if (direction == Vector2.zero) curSpeed = 0.0f;
        else if (curSpeed == 0.0f) curSpeed = initSpeed;
        else curSpeed = Mathf.MoveTowards(curSpeed, maxSpeed, moveAccel * Time.fixedDeltaTime);

        velocity.x = projectedMoveDir.x * curSpeed;
        velocity.z = projectedMoveDir.z * curSpeed;

        if (!cc.isGrounded) velocity.y += gravity * Time.fixedDeltaTime;
        else velocity.y = CheckJump();
    }
    private Vector3 ProjectedMoveDirection()
    {
        Vector3 cameraFwd = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraFwd.y = 0;
        cameraRight.y = 0;

        cameraFwd.Normalize();
        cameraRight.Normalize();

        return cameraFwd * direction.y + cameraRight * direction.x;
    }
    #endregion

    #region JumpCalculations
    float CheckJump() => jumpPressed ? initialJumpVelocity : -cc.skinWidth;
    void CalculateJumpVariables()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initialJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    void JumpForceChange()
    {
        jumpHeight *= 5f;
        CalculateJumpVariables();
    }
    #endregion

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        WeaponBase weapon = hit.collider.GetComponent<WeaponBase>();
        if (weapon != null)
        {
            if (equippedWeapon == null)
            {
                equippedWeapon = weapon;
                equippedWeapon.Equip(GetComponent<Collider>(), weaponAttachPoint1);
            }
            else if (holsteredWeapon == null && weapon != equippedWeapon)
            {
                holsteredWeapon = weapon;
                holsteredWeapon.Equip(GetComponent<Collider>(), weaponAttachPoint2);
            }
            Debug.Log($"Picked up a weapon! {weapon.name}");
        }

        if (hit.gameObject.CompareTag("DeathBall"))
        {
            GameManager.Instance.lives -= 1;
            Debug.Log("Collided with DeathBall. Lost a life.");

            // Add this line to trigger death
            OnDeath();
        }

        // PowerUp logic...
    }
    public void OnSwitchWeapons()
    {
        // Weapon swapping functionality removed as requested.
        // You can add other logic here if needed.
    }
    public void ShootEquippedWeapon()
    {
        if (equippedWeapon != null && equippedWeapon.projectilePrefab != null && equippedWeapon.shootOrigin != null)
        {
            GameObject proj = Instantiate(
                equippedWeapon.projectilePrefab,
                equippedWeapon.shootOrigin.position,
                equippedWeapon.shootOrigin.rotation
            );

            Projectile projectileScript = proj.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                // Choose a speed appropriate for the weapon
                float speed = 15f; // Example: you can expose this per weapon
                Vector3 velocity = equippedWeapon.shootOrigin.forward * speed;
                projectileScript.SetVelocity(velocity);
            }
        }
    }
    public void SaveGamePrepare()
    {
        // Get Player Data Object
        LoadSaveManager.GameStateData.DataPlayer data = GameManager.StateManager.gameStateData.player;

        // Fill in player data for save game
        data.health = health;
        data.cash = cash;
        data.hasWeapon = collectWeapon;

        data.transform.posX = transform.position.x;
        data.transform.posY = transform.position.y;
        data.transform.posZ = transform.position.z;
        data.transform.rotX = transform.rotation.eulerAngles.x;
        data.transform.rotY = transform.rotation.eulerAngles.y;
        data.transform.rotZ = transform.rotation.eulerAngles.z;
        data.transform.scaleX = transform.localScale.x;
        data.transform.scaleY = transform.localScale.y;
        data.transform.scaleZ = transform.localScale.z;
    }

    // Function called when loading is complete
    public void LoadGameComplete()
    {
        // Get Player Data Object
        LoadSaveManager.GameStateData.DataPlayer data = GameManager.StateManager.gameStateData.player;

        //Load data back to Player
        health = data.health;
        cash = data.cash;
        collectWeapon = data.hasWeapon;

        //Give player weapon, activate and destroy weapon power-up
        if (collectWeapon)
        {
            //Find weapon powerup in level
            GameObject weapon = GameObject.Find("WeaponPowerUp01");

            //Send OnTriggerEnter message
            weapon.SendMessage("OnTriggerEnter2D", GetComponent<Collider2D>(), SendMessageOptions.DontRequireReceiver);
        }

        //Set position
        transform.position = new Vector3(data.transform.posX, data.transform.posY, data.transform.posZ);

        //Set rotation
        transform.rotation = Quaternion.Euler(data.transform.rotX, data.transform.rotY, data.transform.rotZ);

        //Set scale
        transform.localScale = new Vector3(data.transform.scaleX, data.transform.scaleY, data.transform.scaleZ);
    }
}
