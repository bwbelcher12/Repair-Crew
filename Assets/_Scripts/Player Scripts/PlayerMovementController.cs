using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] InputActionAsset inputActions;

    public float sensitivity = 1f;
    public float baseSpeed;
    public float sprintSpeed;
    public float walkSpeed;
    public float breakingForce;
    private float playerSpeed;

    private NetworkTransformReliable networkTransform;

    private CharacterController controller;
    private PlayerInputActions playerInputActions;
    private NoClip noClip;

    private Vector3 playerVelocity;
    public bool groundedPlayer;
    public float jumpHeight = 1f;

    private const float gravityConst = -15f;
    private const float noClipConst = 0f;

    private float gravityValue = -15f;
    public float GravityValue 
    { 
        get => gravityValue;
        set => gravityValue = value;
    }

    private bool movementDisabled;
    public bool MovementDisabled
    {
        get => movementDisabled;
        set => movementDisabled = value;
    }

    private Quaternion playerRotation;
    private float cameraTilt;
    private float cameraPan;
    private Vector3 cameraEulerAngles;
    private Vector2 inputForce;

    private Vector2 forceVector;
    private Vector2 previousVector;
    public List<Vector2> last10Vectors = new List<Vector2>();
    public Vector2 rollingAverage;
    
    private bool hitDetect;
    private Collider hitCollider;
    private RaycastHit hit;



    public Camera playerCamera;

    private void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        controller = transform.GetComponent<CharacterController>();
        networkTransform = transform.GetComponent<NetworkTransformReliable>();
        noClip = transform.GetComponent<NoClip>();
        inputActions.FindActionMap("Menus").FindAction("OpenEscapeMenu").performed += DisableMovement;

        if (networkTransform.isOwned)
        {
            playerCamera.enabled = enabled;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!networkTransform.isOwned)
        {
            return;            
        }
        
        //Disable responses to input if the escape menu is open. 
        if (!movementDisabled)
        {
            MoveCamera();
            Move();
            Jump();
        }
        
        //Prevent the player from hovering if they open the escape menu.
        ApplyGravity();
    }

    private void MoveCamera()
    {
        Vector2 mouseMovement = playerInputActions.Player.FirstPersonCameraMove.ReadValue<Vector2>();
        cameraEulerAngles = playerCamera.transform.rotation.eulerAngles;
        SetCameraRotation(mouseMovement.y, mouseMovement.x);
    }

    private void SetCameraRotation(float tiltInput, float panInput)
    {
        cameraTilt -= (tiltInput * sensitivity * -1);
        cameraTilt = Mathf.Clamp(cameraTilt, -90f, 90f);
        cameraPan -= (panInput * sensitivity * -1);
        playerCamera.transform.eulerAngles = new Vector3(cameraTilt, playerCamera.transform.eulerAngles.y, 0f);
        controller.transform.eulerAngles = new Vector3(transform.eulerAngles.x, cameraPan, 0f);
    }

    private void Move()
    {
        inputForce = playerInputActions.Player.Move.ReadValue<Vector2>();

        playerRotation = playerCamera.transform.rotation;

        if (playerInputActions.Player.Walk.ReadValue<float>() == 1)
        {
            playerSpeed = walkSpeed;
        }
        else if (playerInputActions.Player.Sprint.ReadValue<float>() == 1)
        {
            playerSpeed = sprintSpeed;
        }
        else
        {
            playerSpeed = baseSpeed;
        }

        if (inputForce.y == 0 && inputForce.x == 0)
        {
            foreach(Vector2 vector in last10Vectors)
            {
                rollingAverage += vector;
            }

            rollingAverage /= last10Vectors.Count;

            if (Mathf.Abs(previousVector.x) <= .0005f && Mathf.Abs(previousVector.y) <= .0005f)
            {
                rollingAverage = Vector2.zero;
                last10Vectors.Clear();
            }
            previousVector = Vector2.zero;
            controller.Move(new Vector3(rollingAverage.x, 0f, rollingAverage.y));
        }
        else
        {
            previousVector = Vector2.zero;
        }

        if (inputForce.y > 0)
        {
            forceVector = CalcuateForceVector(playerSpeed * Mathf.Abs(inputForce.y) * Time.deltaTime, playerRotation.eulerAngles.y);
            previousVector += forceVector;
            controller.Move(new Vector3(forceVector.x, 0f, forceVector.y));
        }
        if (inputForce.y < 0)
        {
            forceVector = CalcuateForceVector(playerSpeed * Mathf.Abs(inputForce.y) * Time.deltaTime, playerRotation.eulerAngles.y + 180f);
            previousVector += forceVector;

            controller.Move(new Vector3(forceVector.x, 0f, forceVector.y));
        }
        if (inputForce.x > 0)
        {
            forceVector = CalcuateForceVector(playerSpeed * Mathf.Abs(inputForce.x) * Time.deltaTime, playerRotation.eulerAngles.y + 90f);
            previousVector += forceVector;

            controller.Move(new Vector3(forceVector.x, 0f, forceVector.y));
        }
        if (inputForce.x < 0)
        {
            forceVector = CalcuateForceVector(playerSpeed * Mathf.Abs(inputForce.x) * Time.deltaTime, playerRotation.eulerAngles.y + 270f);
            previousVector += forceVector;

            controller.Move(new Vector3(forceVector.x, 0f, forceVector.y));
        }
        controller.Move(new Vector3(0f, playerVelocity.y * Time.deltaTime, 0f));

        last10Vectors.Add(previousVector);
        if(last10Vectors.Count > 10)
        {
            last10Vectors.RemoveAt(0);
        }

        groundedPlayer = IsGrounded();
        if (playerVelocity.y < 0 && groundedPlayer)
        {
            playerVelocity.y = 0f;
        }
    }

    private void Jump()
    {
        if (playerInputActions.Player.Jump.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
    }

    private Vector2 CalcuateForceVector(float force, float theta)
    {
        float fX;
        float fY;
        fX = force * Mathf.Sin(theta * Mathf.Deg2Rad);
        fY = force * Mathf.Cos(theta * Mathf.Deg2Rad);
        return new Vector2(fX, fY);
    }

    private void ApplyGravity()
    {
        playerVelocity.y += gravityValue * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        hitDetect = Physics.BoxCast(controller.transform.position, transform.localScale * 0.5f, Vector3.down, out hit, transform.rotation, controller.transform.lossyScale.y * .62f);
        if (hitDetect)
        {
            return true;
        }
            return false;
    }
    
    private void DisableMovement(InputAction.CallbackContext context)
    {
        if(!movementDisabled)
        {
            movementDisabled = true;
        }
        else
        {
            movementDisabled = false;
        }
    }
}

