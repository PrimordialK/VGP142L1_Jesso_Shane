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

    //Movement variables
    Vector3 gravity = Physics.gravity;
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
        #region Exception 1: Check if PlayerController script is attached to Player GameObject
        var player = GameObject.FindWithTag("Player");
        if (player == null || player.GetComponent<PlayerController>() == null)
        {
            Debug.LogError("PlayerController script is missing from the Player GameObject!");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            #endregion
        }

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

        //Vector3 moveVel = new Vector3(hInput * speed, Physics.gravity.y, vInput * speed);

        //moveVel *= Time.deltaTime;


        //cc.Move(moveVel);

        

    }

    void FixedUpdate()
    {
        //apply movement
        Vector3 projectedMoveDir = ProjectedMoveDirection();
        velocity = projectedMoveDir * speed;
        velocity.y = gravity.y;

        velocity *= Time.fixedDeltaTime;

        cc.Move(velocity);

        //apply rotation
        if (direction != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(projectedMoveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
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
}
