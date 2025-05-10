using UnityEngine;
using Mirror;

public class ReturnToMenu : MonoBehaviour
{
    public GameObject confirmReturnMenu;
    public NetworkManager networkManger;

    private void Awake()
    {
        confirmReturnMenu.SetActive(false);
        //networkManger = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void ReturnWarning()
    {
        confirmReturnMenu.SetActive(true);
    }

    public void CancelReturn()
    {
        confirmReturnMenu.SetActive(false);
    }

    public void GoToMenu()
    {
        if (!NetworkServer.activeHost)
        {
            networkManger.StopClient();
        }
        else
        {
            networkManger.StopHost();
            Debug.Log("hello");
        }
    }
}
