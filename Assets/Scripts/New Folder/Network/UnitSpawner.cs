using Unity.Netcode;
using UnityEngine;

public class UnitSpawner : NetworkBehaviour
{
    public GameObject unitTeamAPrefab;
    public GameObject unitTeamBPrefab;
    public GameObject workerTeamAPrefab;
    public GameObject workerTeamBPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        }
    }

    void HandleClientConnected(ulong clientId)
    {
        int team = clientId == NetworkManager.Singleton.LocalClientId ? NetworkManagerUI.selectedTeam : 1;

        GameObject unitPrefab = team == 0 ? unitTeamAPrefab : unitTeamBPrefab;
        GameObject workerPrefab = team == 0 ? workerTeamAPrefab : workerTeamBPrefab;

        Vector3 spawnPos = team == 0 ? new Vector3(-3, 0, 0) : new Vector3(3, 0, 0);

        var unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        unit.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        var worker = Instantiate(workerPrefab, spawnPos + Vector3.right * 1.5f, Quaternion.identity);
        worker.GetComponent<NetworkObject>().Spawn(true);
    }
}