using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Mirror;

public class HostLobby : MonoBehaviour
{
    [SerializeField] NetworkManager networkManagerObject;
    public void StartLoadCoroutine()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("LobbyScene");

        while(!loadScene.isDone)
        {
            yield return null;
        }

        networkManagerObject.GetComponent<SteamLobby>().HostLobby();
    }
}
