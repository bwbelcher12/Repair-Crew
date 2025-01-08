using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class InteractionManager : NetworkBehaviour
{
    [SerializeField] float playerReach = 2f;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] Camera playerCam;

    RaycastHit hit;
    InteractableObject currentInteractable;

    private void Awake()
    {
        inputActions.FindActionMap("Player").FindAction("Interact").performed += CallInteraction;

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

    private void CallInteraction(InputAction.CallbackContext context)
    {
        currentInteractable.Interact();
    }

    private void OnDestroy()
    {
        inputActions.FindActionMap("Player").FindAction("Interact").performed -= CallInteraction;

    }
}
