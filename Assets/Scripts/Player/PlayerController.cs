using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{

    private InputSystem_Actions input;                  // Source code representation of asset.
    private CharacterController cc;
    private Camera mainCamera;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5.0f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float timeToJumpApex = 0.4f;

    //Movement variables
    float gravity;
    float initialJumpVelocity;

    Vector2 direction; //direction of movement - no gravity is applied here
    Vector3 velocity;

    bool jumpPressed = false;

    void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.SetCallbacks(this);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            cc = GetComponent<CharacterController>();
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
    }

    
    void OnValidate()
    {
        CalculateJumpVariables();
    }

    void OnEnable()
    { 
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }

    void OnDestroy()
    {
        input.Dispose();
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context) => jumpPressed = context.ReadValueAsButton();

    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            direction = context.ReadValue<Vector2>();
            return;
        }
        
        direction = Vector2.zero;
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    
    // Update is called once per frame
    void Update()
    {
        int raysPerAxis = 12; // Number of rays per axis (total rays = raysPerAxis * raysPerAxis)
        float coneAngle = 45f; // Half-angle of the cone in degrees
        float maxDistance = 30f;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        for (int y = 0; y < raysPerAxis; y++)
        {
            // Vertical angle from -coneAngle to +coneAngle
            float verticalAngle = -coneAngle + (2 * coneAngle) * ((float)y / (raysPerAxis - 1));
            Quaternion verticalRot = Quaternion.AngleAxis(verticalAngle, transform.right);

            for (int x = 0; x < raysPerAxis; x++)
            {
                // Horizontal angle from -coneAngle to +coneAngle
                float horizontalAngle = -coneAngle + (2 * coneAngle) * ((float)x / (raysPerAxis - 1));
                Quaternion horizontalRot = Quaternion.AngleAxis(horizontalAngle, transform.up);

                // Combine rotations to get direction
                Vector3 direction = horizontalRot * verticalRot * forward;

                Debug.DrawRay(origin, direction * maxDistance, Color.cyan, 0.1f);

                // Raycast for detection
                if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, LayerMask.GetMask("Enemy")))
                {
                    Debug.Log("Enemy detected in 3D cone: " + hit.collider.name);
                }
            }
        }
    }

    void FixedUpdate()
    {
        //apply movement
        Vector3 projectedMoveDir = ProjectedMoveDirection();
        UpdateCharacterVelocity(projectedMoveDir);

        cc.Move(velocity * Time.fixedDeltaTime);

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
        velocity.x = projectedMoveDir.x * speed;
        velocity.z = projectedMoveDir.z * speed;

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
    #endregion

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.GetMask("Enemy"))
        {
            Debug.Log("Hit an enemy!");
        }
    }
}
