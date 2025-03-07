using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Mirror;
public class PlayerTeleporter : NetworkBehaviour
{
    [SerializeField] private List<NetworkStartPosition> teleportPositions;

    private const string StartRoomName = "PlayerStartingRoom(Clone)";
    private GameObject startRoom;

    private bool foundStartRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LookForStartRoom());
        foundStartRoom = false;

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            StartCoroutine(StartTP());
        }

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MainScene")
        {
            StartCoroutine(StartTP());
        }
    }

    IEnumerator LookForStartRoom()
    {
        while(GameObject.Find(StartRoomName) == null)
        {
            yield return null;
        }
        foundStartRoom = true;
        startRoom = GameObject.Find(StartRoomName);
        AddStartPositions();
        StopCoroutine(LookForStartRoom());
    }

    private void AddStartPositions()
    {
        foreach (Transform child in startRoom.transform)
        {
            if (child.GetComponent<NetworkStartPosition>() != null)
            {
                teleportPositions.Add(child.GetComponent<NetworkStartPosition>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && teleportPositions.Count > 0)
        {
            StartCoroutine(StartTP());
        }
    }

    private void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }

    IEnumerator ControllerEnableDelay()
    {
        yield return new WaitForSeconds(.5f);
        transform.GetComponent<PlayerMovementController>().enabled = true;
        StopCoroutine(ControllerEnableDelay());
    }

    IEnumerator StartTP()
    {
        while(!foundStartRoom)
        {
            yield return null;
        }

        transform.GetComponent<PlayerMovementController>().enabled = false;
        TeleportTo(teleportPositions[Random.Range(0, teleportPositions.Count - 1)].transform.position);
        StartCoroutine(ControllerEnableDelay());
    }
}
