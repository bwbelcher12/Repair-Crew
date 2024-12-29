using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
public class PlayerTeleporter : NetworkBehaviour
{
    [SerializeField] private List<NetworkStartPosition> teleportPositions;

    private const string StartRoomName = "PlayerStartingRoom(Clone)";
    private GameObject startRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LookForStartRoom());

        
    }

    IEnumerator LookForStartRoom()
    {
        while(GameObject.Find(StartRoomName) == null)
        {
            yield return null;
        }
        Debug.Log("start room found");
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
            transform.GetComponent<PlayerMovementController>().enabled = false;
            TeleportTo(teleportPositions[Random.Range(0, teleportPositions.Count - 1)].transform.position);
            StartCoroutine(ControllerEnableDelay());
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
}
