/*
using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public List<Building> buildings = new List<Building>(); // เก็บอาคารที่ถูกสร้างขึ้นมาแล้ว
    public GameObject objBuilding; // Prefab อาคารที่จะใช้ในการสร้าง


    /*public bool CanPlaceBuilding(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 1f); // ตรวจสอบพื้นที่รัศมี 1 หน่วย
        return hit == null; // ถ้าไม่มี Collider อยู่แสดงว่าวางได้
    }#1#
    public void ConstructBuilding(int team, Vector2 position)
    {
        if (objBuilding == null)
        {
            Debug.LogWarning("ไม่มี Prefab อาคารที่ต้องสร้าง!");
            return;
        }

        // สร้างอาคารใหม่จาก Prefab
        GameObject newBuilding = Instantiate(objBuilding, position, Quaternion.identity);
        Building building = newBuilding.GetComponent<Building>();

        if (building != null)
        {
            building.team = team; // ตั้งค่าทีมของอาคาร
            buildings.Add(building); // เพิ่มอาคารลงใน List ของอาคารที่สร้างแล้ว
        }
        else
        {
            Debug.LogError("Prefab ที่สร้างไม่มีคอมโพเนนต์ Building!");
        }
    }


}
*/

using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : NetworkBehaviour
{
    public GameObject objBuilding;
    public List<Building> buildings = new List<Building>();

    public void RequestBuild(int team, Vector2 position)
    {
        ConstructBuildingServerRpc(team, position);
    }

    [ServerRpc]
    private void ConstructBuildingServerRpc(int team, Vector2 position)
    {
        GameObject newBuilding = Instantiate(objBuilding, position, Quaternion.identity);
        newBuilding.GetComponent<NetworkObject>().Spawn();

        Building building = newBuilding.GetComponent<Building>();
        if (building != null)
        {
            building.team = team;
            buildings.Add(building);
        }
    }
}

