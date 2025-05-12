using TMPro;
using UnityEngine;
using System.Collections.Generic;


public class ResourceManager : MonoBehaviour
{
    public int playerAResources = 500; // ทรัพยากรเริ่มต้นของผู้เล่น A
    public int playerBResources = 500; // ทรัพยากรเริ่มต้นของผู้เล่น B

    // อ้างอิงถึง TextMeshPro บน UI
    public TextMeshProUGUI playerAResourceText; // UI แสดงทรัพยากรของผู้เล่น A
    public TextMeshProUGUI playerBResourceText; // UI แสดงทรัพยากรของผู้เล่น B
    
    public static ResourceManager instance; // Singleton เพื่อให้ทุกสคริปต์เข้าถึงได้ง่าย
    
    private Dictionary<int, int> woodByTeam = new Dictionary<int, int>();


    private void Awake()
    {
        if (instance == null) instance = this;
    }
    
    private void Start()
    {
        Debug.Log("Player A Resources: " + playerAResources);
        Debug.Log("Player B Resources: " + playerBResources);
        UpdateResourceUI(); // เรียกใช้งานตอนเริ่มเกม
    }
    
    // ฟังก์ชันเพิ่มทรัพยากรให้กับผู้เล่น A
    public void AddResources(int amount)
    {
        playerAResources += amount;  // เพิ่มทรัพยากร
        UpdateResourceUI();  // อัพเดต UI
    }
    
    public bool SpendResources(int team, int amount)
    {
        Debug.Log("เรียกใช้ SpendResources() | team: " + team + " | amount: " + amount);

        if (team == 0 && playerAResources >= amount)
        {
            playerAResources -= amount;
            Debug.Log("Player A Resources ลดลงเป็น: " + playerAResources);
            UpdateResourceUI();
            return true;
        }
        else if (team == 1 && playerBResources >= amount)
        {
            playerBResources -= amount;
            Debug.Log("Player B Resources ลดลงเป็น: " + playerBResources);
            UpdateResourceUI();
            return true;
        }

        Debug.Log("ใช้ทรัพยากรไม่สำเร็จ");
        return false;
    }
    
    // ฟังก์ชันอัพเดต UI การแสดงทรัพยากร
    private void UpdateResourceUI()
    {
        if (playerAResourceText != null)
        {
            playerAResourceText.text = "Player A Resources: " + playerAResources.ToString();  // แสดงจำนวนทรัพยากร
        }
        
        if (playerBResourceText != null)
            playerBResourceText.text = "Player B Resources: " + playerBResources.ToString();
    }
    
    
    // ฟังก์ชันสำหรับการตัดต้นไม้ (เพื่อเพิ่มไม้เข้ากับ playerAResources)
    public void ChopTree(int woodAmount)
    {
        AddResources(woodAmount);  // เพิ่มไม้เข้ากับทรัพยากรของผู้เล่น A
        Debug.Log("ตัดต้นไม้สำเร็จ ได้ไม้: " + woodAmount);
    }
}