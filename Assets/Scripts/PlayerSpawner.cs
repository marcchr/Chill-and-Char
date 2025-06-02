using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;
using Unity.Cinemachine;
public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefabA;
    public GameObject playerPrefabB;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 0);
        else
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 1);
    }

    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ulong clientId, int prefabId)
    {
        GameObject newPlayer;
        if (prefabId == 0)
            newPlayer = (GameObject)Instantiate(playerPrefabA);
        else
            newPlayer = (GameObject)Instantiate(playerPrefabB);

        newPlayer.transform.position = newPlayer.GetComponent<PlayerMovement>().resetPosition;

        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
    }

}