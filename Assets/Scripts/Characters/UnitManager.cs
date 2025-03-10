using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    public List<Unit> playerAUnits; // รายการหน่วยของผู้เล่น A
    public List<Unit> playerBUnits; // รายการหน่วยของผู้เล่น B

    // ฟังก์ชันสำหรับการสร้างหน่วยใหม่
    public void SpawnUnit(int team, GameObject unitPrefab, Vector2 position)
    {
        GameObject newUnit = Instantiate(unitPrefab, position, Quaternion.identity); // สร้างหน่วยใหม่จาก Prefab ที่ระบุและกำหนดตำแหน่ง
        Unit unit = newUnit.GetComponent<Unit>(); // ดึงคอมโพเนนต์ Unit จากอ็อบเจ็กต์ใหม่
        unit.team = team; // กำหนดทีมให้กับหน่วย (ทีม 0 สำหรับผู้เล่น A, ทีม 1 สำหรับผู้เล่น B)
        
        // ถ้าเป็นทีม A เพิ่มหน่วยไปยังรายการหน่วยของผู้เล่น A
        if (team == 0) playerAUnits.Add(unit);
        // ถ้าเป็นทีม B เพิ่มหน่วยไปยังรายการหน่วยของผู้เล่น B
        else if (team == 1) playerBUnits.Add(unit);
    }
}