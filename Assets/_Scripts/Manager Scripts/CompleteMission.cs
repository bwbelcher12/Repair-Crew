using UnityEngine;

public class CompleteMission : MonoBehaviour
{
    GameFlowManager gameFlowManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameFlowManager = GameObject.Find("GameFlowManager").GetComponent<GameFlowManager>();
    }

    public void Complete()
    {
        gameFlowManager.GoToLobby();
    }
}
