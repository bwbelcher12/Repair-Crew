using UnityEngine;
using Mirror;

public class DoorController : NetworkBehaviour
{
    private bool doorOpen;

    [Command]
    private void CmdToggleDoor()
    {
        if(isServer)
        {
            RpcToggleDoor();
            return;
        }
    }

    [ClientRpc]
    private void RpcToggleDoor()
    {

    }
}
