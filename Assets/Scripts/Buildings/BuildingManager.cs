using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : NetworkBehaviour
{
    public GameObject objBuilding;
    
    public GameObject teamABuildingPrefab;
    public GameObject teamBBuildingPrefab;
    
    public List<Building> buildings = new List<Building>();
    public static BuildingManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void RequestBuild(int team, Vector2 position)
    {
        // เรียกฝั่ง Server สร้าง Building
        Debug.Log($"[Client] RequestBuild จากทีม {team}, Pos: {position}");

        var netObj = GetComponent<NetworkObject>();
        Debug.Log($"[Client] BuildingManager NetworkObject? {(netObj != null)}, Spawned: {netObj?.IsSpawned}");

        ConstructBuildingServerRpc(team, position);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ConstructBuildingServerRpc(int team, Vector2 position, ServerRpcParams rpcParams = default)//private void ConstructBuildingServerRpc(int team, Vector2 position)
    {
        Debug.Log($"[ServerRpc] สร้าง Building ทีม {team} ที่ {position}");
        Debug.Log($"[ServerRpc] Client {rpcParams.Receive.SenderClientId} ขอสร้าง Building ของทีม {team} ที่ {position}");

        GameObject prefabToUse = team == 0 ? teamABuildingPrefab : teamBBuildingPrefab;
        if (prefabToUse == null)
        {
            Debug.LogError("Prefab ของทีมยังไม่ถูกตั้งค่า!");
            return;
        }

        GameObject newBuilding = Instantiate(prefabToUse, position, Quaternion.identity);
        Debug.Log($"สร้างแล้ว: {newBuilding.name}, มี NetworkObject? {newBuilding.GetComponent<NetworkObject>() != null}");

        NetworkObject netObj = newBuilding.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
            Debug.Log($"Spawn สำเร็จ: {newBuilding.name}");
        }
        else
        {
            Debug.LogError("Prefab นี้ไม่มี NetworkObject!!!");
        }
    }
}
