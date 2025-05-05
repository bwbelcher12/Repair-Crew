using UnityEngine;
using Mirror;

public class DoorController : InteractableObject
{
    [Command]
    public new void Interact()
    {
        Debug.Log("Hello connection " + connectionToClient);
    }
}
