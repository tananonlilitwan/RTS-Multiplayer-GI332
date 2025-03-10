using UnityEngine;
using System.Collections.Generic;

/*public class BuildingManager : MonoBehaviour
{
    public List<Building> buildings; // รายการของอาคารทั้งหมดในเกม

    // ฟังก์ชันสำหรับการสร้างอาคารใหม่
    public void ConstructBuilding(int team, GameObject buildingPrefab, Vector2 position)
    {
        GameObject newBuilding = Instantiate(buildingPrefab, position, Quaternion.identity); // สร้างอาคารใหม่จาก Prefab ที่ระบุและกำหนดตำแหน่ง
        Building building = newBuilding.GetComponent<Building>(); // ดึงคอมโพเนนต์ Building จากอ็อบเจ็กต์ใหม่
        building.team = team; // กำหนดทีมให้กับอาคาร (ทีม 0 สำหรับผู้เล่น A, ทีม 1 สำหรับผู้เล่น B)
        buildings.Add(building); // เพิ่มอาคารที่สร้างขึ้นในรายการของอาคารทั้งหมด
    }
}*/


public class BuildingManager : MonoBehaviour
{
    public List<Building> buildings = new List<Building>(); // เก็บอาคารที่ถูกสร้างขึ้นมาแล้ว
    public GameObject objBuilding; // Prefab อาคารที่จะใช้ในการสร้าง

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
