using UnityEngine;
using System.Collections.Generic;

public class RTSGameManager : MonoBehaviour
{
    public static RTSGameManager Instance; // ตัวแปรสำหรับเก็บอินสแตนซ์ของ GameManager

    public List<TerritoryPoint> territoryPoints; // รายการของพื้นที่ที่สามารถถูกยึด
    public Base headquartersA; // ฐานของผู้เล่น A
    public Base headquartersB; // ฐานของผู้เล่น B
    public ResourceManager resourceManager; // ตัวจัดการทรัพยากร
    public UnitManager unitManager; // ตัวจัดการหน่วยทหาร
    public BuildingManager buildingManager; // ตัวจัดการอาคาร

    private void Awake()
    {
        if (Instance == null) // ถ้ายังไม่มีอินสแตนซ์ของ GameManager
            Instance = this; // กำหนดให้ตัวเองเป็นอินสแตนซ์เดียว
    }

    private void Update()
    {
        CheckWinCondition(); // ตรวจสอบเงื่อนไขการชนะในทุกๆ เฟรม
    }

    private void CheckWinCondition()
    {
        int playerACaptured = 0; // จำนวนพื้นที่ที่ผู้เล่น A ยึดได้
        int playerBCaptured = 0; // จำนวนพื้นที่ที่ผู้เล่น B ยึดได้
        
        foreach (var point in territoryPoints) // ลูปผ่านทุกพื้นที่
        {
            if (point.currentOwner == 0) playerACaptured++; // ถ้าผู้เล่น A เป็นเจ้าของพื้นที่ เพิ่มจำนวนพื้นที่ที่ผู้เล่น A ยึด
            if (point.currentOwner == 1) playerBCaptured++; // ถ้าผู้เล่น B เป็นเจ้าของพื้นที่ เพิ่มจำนวนพื้นที่ที่ผู้เล่น B ยึด
        }

        if (playerACaptured >= 2) // ถ้าผู้เล่น A ยึดพื้นที่ได้มากกว่าหรือเท่ากับ 2 แห่ง
        {
            Debug.Log("Player A Wins!"); // ผู้เล่น A ชนะ
        }
        else if (playerBCaptured >= 2) // ถ้าผู้เล่น B ยึดพื้นที่ได้มากกว่าหรือเท่ากับ 2 แห่ง
        {
            Debug.Log("Player B Wins!"); // ผู้เล่น B ชนะ
        }
        else if (headquartersA.health <= 0) // ถ้าฐานของผู้เล่น A ถูกทำลาย
        {
            Debug.Log("Player B Wins! (HQ Destroyed)"); // ผู้เล่น B ชนะเพราะทำลายฐานของผู้เล่น A
        }
        else if (headquartersB.health <= 0) // ถ้าฐานของผู้เล่น B ถูกทำลาย
        {
            Debug.Log("Player A Wins! (HQ Destroyed)"); // ผู้เล่น A ชนะเพราะทำลายฐานของผู้เล่น B
        }
    }
}
