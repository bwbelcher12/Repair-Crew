using UnityEngine;
using Mirror;

public class DoorController : InteractableObject
{
    [SerializeField] NetworkAnimator n_animator;
    [SerializeField] PlayOneshot doorSound;

    [Command]
    public new void Interact()
    {

    }


    private bool doorState;

    private void Start()
    {
        doorState = n_animator.animator.GetBool("Open");
    }

    public void ToggleDoor()
    {
        n_animator.animator.SetBool("Open", !doorState);
        doorState = !doorState;
        RpcPlaySound();
    }
    
    [ClientRpc]
    public void RpcPlaySound()
    {
        doorSound.PlayClip();
    }
}
