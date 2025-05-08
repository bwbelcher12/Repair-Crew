using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InteractionManager : NetworkBehaviour
{
    [SerializeField] float playerReach = 2f;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] Camera playerCam;
    [SerializeField] GameObject crosshair;
    [SerializeField] GameObject progressBar;

    RaycastHit hit;
    public float timer;
    private const float barMax = .2f;
    RectTransform progressBarTransform;
    PlayerInputActions playerInputActions;


    [SerializeField] InteractableObject currentInteractable;

    private void Start()
    {
        if (!isLocalPlayer)
            return;
        progressBarTransform = progressBar.GetComponent<RectTransform>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        //inputActions.FindActionMap("Player").FindAction("Interact").performed += CmdCallInteraction;
        progressBar.SetActive(false);
    }
    void FixedUpdate()
    {
        CheckInteraction();
    }

    private void CheckInteraction()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        if(Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.tag == "Interactable")
            {
                InteractableObject newInteractabel = hit.collider.GetComponent<InteractableObject>();
                if (newInteractabel.enabled)
                {
                    SetNewInteractable(newInteractabel);
                    crosshair.SetActive(true);
                }
                else //Looking at a disabled interactable object
                {
                    DisableCurrnetInteraction();
                    crosshair.SetActive(false);
                }
            }
            else //Not looking at an interactable object
            {
                DisableCurrnetInteraction();
                crosshair.SetActive(false);
            }
        }
        else //Nothing in range
        {
            DisableCurrnetInteraction();
            crosshair.SetActive(false);
        }

        InteractionDelay();

    }

    private void SetNewInteractable(InteractableObject newInteractable)
    {
        currentInteractable = newInteractable;
    }

    private void DisableCurrnetInteraction()
    {
        if(currentInteractable)
        {
            currentInteractable = null;
        }
    }

    private void InteractionDelay()
    {
        if (!currentInteractable)
        {
            return;
        }

        if(playerInputActions.Player.Interact.ReadValue<float>() == 1f)
        {
            progressBar.SetActive(true);
            progressBarTransform.localScale = new Vector3(timer * .1f, 2f, .2f);
            timer += Time.deltaTime;
            if(timer >= .5f)
            {
                CmdCallInteraction();
                progressBarTransform.sizeDelta = new Vector3(0f, 2f, .2f);
                timer = 0f;
            }
        }
        else
        {
            progressBar.SetActive(false);
            progressBarTransform.sizeDelta = new Vector3(0f, 2f, .2f);
            timer = 0f;
        }
    }

    [Command]
    private void CmdCallInteraction()
    {
        if (!isServer)
        {
            return;
        }
        RpcCallInteraction();
    }

    [ClientRpc]
    private void RpcCallInteraction()
    {
        if (!currentInteractable) //Do nothing if there isn't a current interactable.
            return;


        currentInteractable.CmdInteract();
    }

    private void OnDestroy()
    {
        //inputActions.FindActionMap("Player").FindAction("Interact").performed -= CmdCallInteraction;

        playerInputActions.Player.Disable();

    }
}
