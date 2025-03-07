using UnityEngine;
using Mirror;
public class GameFlowManager : NetworkBehaviour 
{
    private NetworkManager networkManager;
    private SteamLobby steamLobby;

    private const string mainScene = "MainScene";
    private const string lobbyScene = "LobbyScene";

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        steamLobby = GameObject.Find("NetworkManager").GetComponent<SteamLobby>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isServer)
            CMDStartLevel();

        //Debug.Log(NetworkServer.isLoadingScene);
        //Debug.Log(NetworkClient.ready);
    }

    public void StartLevel()
    {
        if (isServer)
        {
            ServerStartLevel();
        }
        else
        {
            CMDStartLevel();
        }
    }

    [Server]
    private void ServerStartLevel()
    {
        networkManager.ServerChangeScene(mainScene);
        steamLobby.ToggleLobbyJoinable(false);
    }

    [Command]
    private void CMDStartLevel()
    {
        ServerStartLevel();
    }

    public void GoToLobby()
    {
        if(isServer)
        {
            ServerGoToLobby();
        }
        else
        {
            CMDGoToLobby();
        }
    }

    [Server]
    private void ServerGoToLobby()
    {
        networkManager.ServerChangeScene(lobbyScene);
    }


    [Command]
    private void CMDGoToLobby()
    {
        ServerGoToLobby();
    }
    
}
