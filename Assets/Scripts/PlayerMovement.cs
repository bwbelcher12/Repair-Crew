using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float jumpForce;
    public float accelerationSpeed;
    public float maxVelocity;
    public float sensitivity;
    public float stepHeight;
    public float centerHeight;
    public float stepSmoothing;
    //public GameObject upperStepChecker;
    //public GameObject lowerStepChecker;
    //public GameObject stepCheckerAnchor;

    float cameraTilt;
    float cameraPan;
    float modifiedAccelerationSpeed;
    public bool grounded;
    bool jumpBuffer;


    Quaternion playerRotation;
    Vector3 cameraEulerAngles;
    Vector2 inputForce;

    PlayerInputActions playerInputActions;
    public Camera playerCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        //stepCheckerAnchor.transform.localPosition = new Vector3(0f, stepHeight - 1f, 0f);
    }

    private void Update()
    {
        Vector2 mouseMovement = playerInputActions.Player.FirstPersonCameraMove.ReadValue<Vector2>();
        cameraEulerAngles = playerCamera.transform.rotation.eulerAngles;
        SetCameraRotation(mouseMovement.y, mouseMovement.x);
        inputForce = playerInputActions.Player.Move.ReadValue<Vector2>();


        modifiedAccelerationSpeed = accelerationSpeed;
        if (!grounded)
        {
            modifiedAccelerationSpeed *= .1f;
        }
    }

    private void FixedUpdate()
    {
        Move();
        BreakingForce();
        //CheckForStep();
        KeepGrounded();
        
    }

    private void SetCameraRotation(float tiltInput, float panInput)
    {
        cameraTilt -= (tiltInput * sensitivity * -1);
        cameraTilt = Mathf.Clamp(cameraTilt, -90f, 90f);
        cameraPan -= (panInput * sensitivity * -1);
        playerCamera.transform.eulerAngles = new Vector3(cameraTilt, cameraPan, 0f);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(!grounded)
        {
            return;
        }
        if(context.performed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
            StartCoroutine(ApplyJumpBuffer());
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

    private void OnCollisionStay(Collision collision)
    {
        if(jumpBuffer)
        {
            return;
        }
        grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
    private void OnCollisionEnter(Collision collision)
    {

    }

    private void Move()
    {
        transform.eulerAngles = new Vector3(0f, cameraEulerAngles.y, 0f);
        playerRotation = rb.transform.rotation;

        //MoveStepChecker(Vector2.zero);

        if (inputForce.y > 0)
        {
            Vector2 forceVector = CalcuateForceVector(modifiedAccelerationSpeed * Mathf.Abs(inputForce.y), playerRotation.eulerAngles.y);
            rb.AddForce(new Vector3(forceVector.x, 0f, forceVector.y), ForceMode.Acceleration);
            //MoveStepChecker(forceVector);
        }
        if (inputForce.y < 0)
        {
            Vector2 forceVector = CalcuateForceVector(modifiedAccelerationSpeed * Mathf.Abs(inputForce.y), playerRotation.eulerAngles.y + 180f);
            rb.AddForce(new Vector3(forceVector.x, 0f, forceVector.y), ForceMode.Acceleration);
            //MoveStepChecker(forceVector);
        }
        if (inputForce.x > 0)
        {
            Vector2 forceVector = CalcuateForceVector(modifiedAccelerationSpeed * Mathf.Abs(inputForce.x), playerRotation.eulerAngles.y + 90);
            rb.AddForce(new Vector3(forceVector.x, 0f, forceVector.y), ForceMode.Acceleration);
            //MoveStepChecker(forceVector);
        }
        if (inputForce.x < 0)
        {
            Vector2 forceVector = CalcuateForceVector(modifiedAccelerationSpeed * Mathf.Abs(inputForce.x), playerRotation.eulerAngles.y + 270);
            rb.AddForce(new Vector3(forceVector.x, 0f, forceVector.y), ForceMode.Acceleration);
            //MoveStepChecker(forceVector);
        }
        //Debug.Log(rb.velocity);
    }

    private void BreakingForce()
    {
        //Apply a breaking force if speed is too high, doesn't affect vertical velocity.
        if ((rb.velocity.sqrMagnitude > maxVelocity) || inputForce == Vector2.zero)
        { 
            rb.AddForce((rb.velocity.sqrMagnitude - maxVelocity) * -1f * new Vector3(rb.velocity.x, 0f, rb.velocity.z), ForceMode.Force);
            
        }
        
    }

    private void KeepGrounded()
    {

        RaycastHit hit;

        if(Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            centerHeight = hit.distance;
            Debug.Log(hit.distance);
            if (hit.distance > (1.45f + stepHeight) && hit.distance < 1.45f +stepHeight + .2f &&grounded)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - hit.distance + .11f, transform.position.z);
            }
        }
    }

    /*
    private void MoveStepChecker(Vector2 positions)
    {
        Vector3 newRotation = new Vector3(positions.x, 0, positions.y);

        if(newRotation == Vector3.zero)
        {
            return;
        }

        stepChecker.transform.localPosition = new Vector3(positions.y * -.1f, stepChecker.transform.localPosition.y, positions.x * -.1f);
        //stepCheckerAnchor.transform.rotation = Quaternion.LookRotation(new Vector3(rb.velocity.x, 0, rb.velocity.z));
    }

    private void CheckForStep()
    {
        RaycastHit lowerHit;
        if (Physics.Raycast(lowerStepChecker.transform.position, new Vector3(rb.velocity.x, 0f, rb.velocity.z), out lowerHit, .1f))
        {
            Debug.DrawLine(lowerStepChecker.transform.position, lowerHit.point, Color.red);
            RaycastHit upperHit;
            if(!Physics.Raycast(upperStepChecker.transform.position, new Vector3(rb.velocity.x, 0f, rb.velocity.z), out upperHit, .2f))
            {
                rb.position -= new Vector3(0f, -stepSmoothing, 0f);
                Debug.DrawLine(upperStepChecker.transform.position, upperHit.point, Color.red);
            }
        }

    }
    */

    IEnumerator ApplyJumpBuffer()
    {
        jumpBuffer = true;

        for(float timer = .1f; timer >= 0; timer -= Time.deltaTime)
        {
            yield return null;
        }
        jumpBuffer = false;
    }

}