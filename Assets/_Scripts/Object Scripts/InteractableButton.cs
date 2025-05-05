using UnityEngine;
using Mirror;

public class InteractableButton : InteractableObject
{
    [SerializeField] MeshCollider buttonCollider;

    public void InteractCalled()
    {
        Debug.Log("Interaction called from " + connectionToClient);
    }
}
