using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;
using System.Collections;

/*
 * CREATED USING THIS TUTORIAL: https://youtu.be/QlbBC07dqnE?si=pn4OVR7YhtQzmFiZ  
 */
public class SteamLobby2 : MonoBehaviour
{
    [SerializeField] GameObject networkManagerObject;
    private NetworkManager networkManager;
    private CSteamID currentLobbyID = new CSteamID();

    private const string HostAddressKey = "HostAdress";
    private const string LobbyScene = "LobbyScene";
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        networkManager = networkManagerObject.GetComponent<NetworkManager>();

        if (!SteamManager.Initialized)
        {
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        StartLoadCoroutine(LobbyScene);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);


        if (NetworkServer.active)
        {
            return;
        }

        string hostAdress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        networkManager.networkAddress = hostAdress;
        networkManager.StartClient();
    }



    public void ExitLobby()
    {
        SteamMatchmaking.LeaveLobby(currentLobbyID);
        currentLobbyID = new CSteamID();

        networkManager.StopHost();
    }


    public void StartLoadCoroutine(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName);

        while (!loadScene.isDone)
        {
            yield return null;
        }

    }
}
