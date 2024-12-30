using UnityEngine;

public class CursorControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    //Toggle cursor between locked and confined
    public void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Confined)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if(Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void SetCursorToLocked()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetCursorToConfined()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetCursorToNone()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}