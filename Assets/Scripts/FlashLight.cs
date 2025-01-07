using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FlashLight : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;

    [SerializeField] Light flashlight;
    [SerializeField] Camera playerCam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions.FindActionMap("Player").FindAction("ToggleFlashlight").performed += ToggleFlashLight;
    }


    private void ToggleFlashLight(InputAction.CallbackContext context)
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
}
