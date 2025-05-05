using UnityEngine.Events;
using UnityEngine;
using Mirror;

public class InteractableObject : NetworkBehaviour
{
    public UnityEvent onInteraction;

    [Command (requiresAuthority = false)]
    public void CmdInteract()
    {
        if (isServer)
        {
            Interact();
            return;
        }
    }

    public void Interact()
    {
        if (!isServer)
            return;

        onInteraction.Invoke();
    }
}
