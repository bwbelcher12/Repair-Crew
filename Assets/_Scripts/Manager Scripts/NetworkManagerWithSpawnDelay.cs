using UnityEngine;
using Mirror;
using System.Collections;

public class NetworkManagerWithSpawnDelay : NetworkManager
{
    float dealay = 5f;
    float elapsedTime = 0f;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        while (elapsedTime <= dealay)
        {
            elapsedTime += Time.deltaTime;
            continue;
        }
        Debug.Log(startPositions.Count);
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

}
