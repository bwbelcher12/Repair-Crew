using UnityEngine;
using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;

/*
 * CREATED USING THIS TUTORIAL: https://youtu.be/QlbBC07dqnE?si=pn4OVR7YhtQzmFiZ  
 */
public class SteamLobby : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject disconnectUI;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private GameObject levelGenerator;
    private NetworkManager networkManager;
    private CSteamID currentLobbyID = new CSteamID();

    private const string HostAddressKey = "HostAdress";

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();

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
        menuUI.SetActive(false);
        disconnectUI.SetActive(true);
        menuCamera.enabled = false;
        menuCamera.GetComponent<AudioListener>().enabled = false;

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        StartCoroutine(GenerateWorldAfterClientReady());
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            menuUI.SetActive(true);
            disconnectUI.SetActive(false);
            return;
        }

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
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

        menuUI.SetActive(false);
        disconnectUI.SetActive(true);
        menuCamera.enabled = false;
        menuCamera.GetComponent<AudioListener>().enabled = false;
        StartCoroutine(DeleteDoorsAfterClientReady());
    }

    IEnumerator GenerateWorldAfterClientReady()
    {
        while (!NetworkClient.ready)
        {
            yield return null;
        }

        NetworkConnectionToClient conn = new NetworkConnectionToClient(NetworkClient.connection.connectionId, clientAddress: "localhost");
        levelGenerator.SetActive(true);
        levelGenerator.GetComponent<LevelGenerator2>().GenerateLevel();
    }

    IEnumerator DeleteDoorsAfterClientReady()
    {
        while (!NetworkClient.ready)
        {
            yield return null;
        }
        levelGenerator.GetComponent<LevelGenerator2>().ServerDestroyExtraWalls();
    }


    public void ExitLobby()
    {
        levelGenerator.GetComponent<LevelGenerator2>().ClearLevel();
        menuUI.SetActive(true);
        menuCamera.enabled = true;
        disconnectUI.SetActive(false);
        SteamMatchmaking.LeaveLobby(currentLobbyID);
        currentLobbyID = new CSteamID();
        networkManager.StopHost();

    }

}
