using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;

    [SerializeField] GameObject escapeMenuCanvas;

    private CursorControl cursorControl = new CursorControl();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions.FindActionMap("Menus").FindAction("OpenEscapeMenu").performed += OpenEscapeMenu;
    }


    private void OpenEscapeMenu(InputAction.CallbackContext context)
    {
        if(escapeMenuCanvas.activeSelf == true)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                return;
            }
            cursorControl.SetCursorToLocked();
        }
        else
        {

            escapeMenuCanvas.SetActive(true);
            
            cursorControl.SetCursorToConfined();
        }
    }
}
