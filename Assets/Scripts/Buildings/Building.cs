/*using UnityEngine;
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
}*/


using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Building : NetworkBehaviour
{
    public int team;
    public TextMeshProUGUI healthTextTMP;

    // ใช้ NetworkVariable แทน int ธรรมดา
    public NetworkVariable<int> health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        health.OnValueChanged += OnHealthChanged;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return; // ให้ Server เป็นผู้จัดการ HP
        health.Value -= damage;
        if (health.Value <= 0)
        {
            health.Value = 0;
            Debug.Log("Base Destroyed!");
            CheckWinCondition(); // ฝั่งที่ชนะหรือแพ้
        }
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateHealthUI(); // อัปเดต UI ทุก client
    }

    private void UpdateHealthUI()
    {
        if (healthTextTMP != null)
        {
            healthTextTMP.text = "Base HP: " + health.Value.ToString();
        }
    }

    private void CheckWinCondition()
    {
        if (!IsServer) return;

        if (team == 0) // Team A แพ้
        {
            GameManager.Instance.TriggerWinPanelForClient(1); // Team B ชนะ (Client)
            GameManager.Instance.TriggerLosePanelForClient(0); // Team A แพ้ (Host)
        }
        else if (team == 1) // Team B แพ้
        {
            GameManager.Instance.TriggerWinPanelForClient(0); // Team A ชนะ (Host)
            GameManager.Instance.TriggerLosePanelForClient(1); // Team B แพ้ (Client)
        }
    }
    
    public void Repair()
    {
        health.Value = Mathf.Min(health.Value + 20, 100);
        Debug.Log(name + " กำลังซ่อมอาคาร, Health เหลือ: " + health.Value);
    }
    
    

}
