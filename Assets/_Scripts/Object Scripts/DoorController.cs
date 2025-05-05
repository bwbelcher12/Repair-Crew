using UnityEngine;
using Mirror;

public class DoorController : InteractableObject
{
    [SerializeField] NetworkAnimator n_animator;
    [SerializeField] PlayOneshot doorSound;

    [Command]
    public new void Interact()
    {
        Debug.Log("Hello connection " + connectionToClient);
    }


    private bool doorState;

    private void Start()
    {
        doorState = n_animator.animator.GetBool("Open");
    }

    public void InteractCalled()
    {
        Debug.Log("Interaction called from " + connectionToClient);
    }

    public void ToggleDoor()
    {
        n_animator.animator.SetBool("Open", !doorState);
        doorState = !doorState;
    }

    [Command]
    public void CmdPlaySound()
    {
        if (isServer)
            return;

        RpcPlaySound();
    }
    
    [ClientRpc]
    public void RpcPlaySound()
    {
        doorSound.PlayClip();
    }
}
