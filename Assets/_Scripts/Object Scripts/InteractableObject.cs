using UnityEngine.Events;
using UnityEngine;
using Mirror;

public class InteractableObject : MonoBehaviour
{
    public UnityEvent onInteraction;

    public void Interact()
    {
        onInteraction.Invoke();
    }
}
