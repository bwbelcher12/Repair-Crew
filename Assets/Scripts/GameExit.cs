using UnityEngine;

public class GameExit : MonoBehaviour
{
    public GameObject confirmExitMenu;

    private void Awake()
    {
        confirmExitMenu.SetActive(false);
    }

    public void ExitWarning()
    {
        confirmExitMenu.SetActive(true);
    }

    public void CancelExit()
    {
        confirmExitMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
