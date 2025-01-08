using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class FlashLight : NetworkBehaviour
{
    [SerializeField] InputActionAsset inputActions;

    [SerializeField] Light flashlight;
    [SerializeField] Camera playerCam;


    void Start()
    {
        if (!isLocalPlayer)
            return;

        inputActions.FindActionMap("Player").FindAction("ToggleFlashlight").performed += CmdToggleFlashlight;
    }

    [Command]
    private void CmdToggleFlashlight(InputAction.CallbackContext context)
    {
        if(isServer)
        {
            RpcToggleFlashlight();
            return;
        }
        if (flashlight.enabled == true)
        {
            flashlight.enabled = false;
        }
        else
        {
            flashlight.enabled = true;
        }
    }

    [ClientRpc]
    private void RpcToggleFlashlight()
    {
        if (flashlight.enabled == true)
        {
            flashlight.enabled = false;
        }
        else
        {
            flashlight.enabled = true;
        }
    }

    private void Update()
    {
        flashlight.transform.rotation = playerCam.transform.rotation; 
    }

    private void OnDestroy()
    {
        inputActions.FindActionMap("Player").FindAction("ToggleFlashlight").performed -= CmdToggleFlashlight;
    }
}
