using UnityEngine;
using TMPro;

public class Building : MonoBehaviour
{
    public int team; // 0 สำหรับผู้เล่น A, 1 สำหรับผู้เล่น B
    public int health = 1000; // ค่าเริ่มต้นของพลังชีวิตฐานคือ 1000
    public TextMeshProUGUI healthTextTMP; // ถ้าใช้ TextMeshPro

    // ฟังก์ชันสำหรับรับความเสียหาย
    public void TakeDamage(int damage)
    {
        health -= damage; // ลบพลังชีวิตของฐานจากความเสียหายที่ได้รับ
        if (health <= 0) // ถ้าพลังชีวิตของฐานเหลือ 0 หรือ น้อยกว่า
        {
            health = 0; // ตั้งค่า health เป็น 0
            Debug.Log("Base Destroyed!"); // แสดงข้อความว่า "ฐานถูกทำลาย"
        }

        UpdateHealthUI(); // อัปเดต UI เมื่อพลังชีวิตเปลี่ยนแปลง
    }

    // ฟังก์ชันอัปเดต UI
    private void UpdateHealthUI()
    {
        if (healthTextTMP != null)
        {
            healthTextTMP.text = "Base HP: " + health.ToString(); // แสดงค่า HP ที่ UI
        }
    }

    private void Start()
    {
        UpdateHealthUI(); // เรียกใช้ตอนเริ่มเกมเพื่อแสดงค่า HP ตอนเริ่ม
    }
    
    // ฟังก์ชันซ่อม
    public void Repair()
    {
        health = Mathf.Min(health + 20, 100);  // ซ่อมแซมให้ไม่เกิน 100
        Debug.Log(name + " กำลังซ่อมอาคาร, Health เหลือ: " + health);
    }
}