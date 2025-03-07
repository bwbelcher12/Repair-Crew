using UnityEngine;

public class ButtonSceneChanger : MonoBehaviour
{

    GameFlowManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameFlowManager").GetComponent<GameFlowManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GoToLobbyScene()
    {
        gameManager.GoToLobby();
    }
}
