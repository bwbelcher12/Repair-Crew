using UnityEngine;
using Mirror;
public class GameFlowManager : NetworkBehaviour 
{
    private NetworkManager networkManager;

    private const string mainScene = "MainScene";

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isServer)
            StartLevel();

        //Debug.Log(NetworkServer.isLoadingScene);
        //Debug.Log(NetworkClient.ready);
    }

    [Server]
    private void StartLevel()
    {
        networkManager.ServerChangeScene(mainScene);
    }



    
}
