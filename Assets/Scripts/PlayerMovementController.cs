using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float sensitivity = 1f;
    public float playerSpeed = .2f;


    private CharacterController controller;
    private PlayerInputActions playerInputActions;
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    public float jumpHeight = 1f;
    private float gravityValue = -15f;

    private Quaternion playerRotation;
    private float cameraTilt;
    private float cameraPan;
    private Vector3 cameraEulerAngles;
    private Vector2 inputForce;
    
    private bool hitDetect;
    private Collider hitCollider;
    private RaycastHit hit;

    public Camera playerCamera;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        controller = transform.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        MoveCamera();

        if (playerInputActions.Player.Jump.triggered && groundedPlayer)
        {
            Jump();
        }

        controller.Move(playerVelocity * Time.deltaTime);

    }

    private void MoveCamera()
    {
        Vector2 mouseMovement = playerInputActions.Player.FirstPersonCameraMove.ReadValue<Vector2>();
        cameraEulerAngles = playerCamera.transform.rotation.eulerAngles;
        SetCameraRotation(mouseMovement.y, mouseMovement.x);
        inputForce = playerInputActions.Player.Move.ReadValue<Vector2>();  
    }

    private void SetCameraRotation(float tiltInput, float panInput)
    {
        cameraTilt -= (tiltInput * sensitivity * -1);
        cameraTilt = Mathf.Clamp(cameraTilt, -90f, 90f);
        cameraPan -= (panInput * sensitivity * -1);
        playerCamera.transform.eulerAngles = new Vector3(cameraTilt, cameraPan, 0f);
    }

    private void Move()
    {
        playerRotation = playerCamera.transform.rotation;

        if (inputForce.y > 0)
        {
            Vector2 forceVector = CalcuateForceVector(playerSpeed * Mathf.Abs(inputForce.y) * Time.deltaTime, playerRotation.eulerAngles.y);
            controller.Move(new Vector3(forceVector.x, 0f, forceVector.y));
        }
        if (inputForce.y < 0)
        {
            Vector2 forceVector = CalcuateForceVector(playerSpeed * Mathf.Abs(inputForce.y) * Time.deltaTime, playerRotation.eulerAngles.y + 180f);
            controller.Move(new Vector3(forceVector.x, 0f, forceVector.y));
        }
        if (inputForce.x > 0)
        {
            Vector2 forceVector = CalcuateForceVector(playerSpeed * Mathf.Abs(inputForce.x) * Time.deltaTime, playerRotation.eulerAngles.y + 90);
            controller.Move(new Vector3(forceVector.x, 0f, forceVector.y));
        }
        if (inputForce.x < 0)
        {
            Vector2 forceVector = CalcuateForceVector(playerSpeed * Mathf.Abs(inputForce.x) * Time.deltaTime, playerRotation.eulerAngles.y + 270);
            controller.Move(new Vector3(forceVector.x, 0f, forceVector.y));
        }
        groundedPlayer = IsGrounded();
        if (playerVelocity.y < 0 && groundedPlayer)
        {
            playerVelocity.y = 0f;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
    }

    private void Jump()
    {
     
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        
    }

    private Vector2 CalcuateForceVector(float force, float theta)
    {
        float fX;
        float fY;
        fX = force * Mathf.Sin(theta * Mathf.Deg2Rad);
        fY = force * Mathf.Cos(theta * Mathf.Deg2Rad);
        return new Vector2(fX, fY);
    }

    private bool IsGrounded()
    {
        hitDetect = Physics.BoxCast(controller.transform.position, transform.localScale * 0.5f, Vector3.down, out hit, transform.rotation, controller.transform.lossyScale.y * .67f);
        if (hitDetect)
        {
            //Output the name of the Collider your Box hit
            return true;
        }
            return false;
    }
}

