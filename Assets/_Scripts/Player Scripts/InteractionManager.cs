using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;


public class InteractionManager : NetworkBehaviour
{
    [SerializeField] float playerReach = 2f;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] Camera playerCam;
    [SerializeField] GameObject crosshair;

    RaycastHit hit;

    [SerializeField] InteractableObject currentInteractable;

    private void Start()
    {
        if (!isLocalPlayer)
            return;

        inputActions.FindActionMap("Player").FindAction("Interact").performed += CmdCallInteraction;
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

    [Command]
    private void CmdCallInteraction(InputAction.CallbackContext context)
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
        inputActions.FindActionMap("Player").FindAction("Interact").performed -= CmdCallInteraction;

    }
}
