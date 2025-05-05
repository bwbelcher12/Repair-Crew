using UnityEngine;
using Mirror;

public class InteractableButton : InteractableObject
{
    [SerializeField] MeshCollider buttonCollider;


    void InteractCalled()
    {
        Debug.Log("Interaction called from " + connectionToServer);
    }
}
