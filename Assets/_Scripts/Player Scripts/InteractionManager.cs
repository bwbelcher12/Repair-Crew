using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class InteractionManager : NetworkBehaviour
{
    [SerializeField] float playerReach = 2f;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] Camera playerCam;

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
                }
                else //Looking at a disabled interactable object
                {
                    DisableCurrnetInteraction();
                }
            }
            else //Not looking at an interactable object
            {
                DisableCurrnetInteraction();
            }
        }
        else //Nothing in range
        {
            DisableCurrnetInteraction();
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
