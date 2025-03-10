using UnityEngine;
using System.Collections.Generic;

public class Tree : MonoBehaviour
{
    public float health = 100f;  // กำหนดค่า maxHealth
    private float maxHealth;           // ค่าพลังชีวิตปัจจุบัน
    public float minX = -10f, maxX = 10f, minY = -10f, maxY = 10f; // ขอบเขตการสุ่ม
    public LayerMask obstacleLayer; // กำหนดว่าอะไรเป็นสิ่งกีดขวาง
    public int woodReward = 50; // จำนวนทรัพยากรไม้ที่จะให้เมื่อไม้ถูกตัดเสร็จ

    
    private void Start()
    {
        maxHealth = health;
    }

    /*public void Chop(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            RelocateTree(); // ย้ายที่ต้นไม้แทนการทำลาย
            ResourceManager.instance.AddWood(woodReward); // เพิ่มไม้ใน ResourceManager
        }
    }*/
    public void Chop(int damage, int team)
    {
        health -= damage;
        if (health <= 0)
        {
            ResourceManager.instance.AddResources(woodReward);  // เพิ่มทรัพยากรไม้ให้กับผู้เล่น
            RelocateTree(); // ย้ายที่ต้นไม้แทนการทำลาย
        }
    }


    public void ResetHealth()
    {
        health = maxHealth;
    }
    public void Chop(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            RelocateTree(); // ย้ายที่ต้นไม้แทนการทำลาย
        }
    }

    private void RelocateTree()
    {
        Vector3 newPosition = GetSafePosition();
        transform.position = newPosition; // ย้ายต้นไม้ไปตำแหน่งใหม่
        health = 50; // รีเซ็ตพลังชีวิต
    }

    private Vector3 GetSafePosition()
    {
        int maxAttempts = 100; // กำหนดจำนวนครั้งที่สุ่มเพื่อป้องกันลูปไม่รู้จบ
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                0f
            );

            // ตรวจสอบว่ามีการชนกับสิ่งกีดขวางหรือไม่
            Collider2D[] colliders = Physics2D.OverlapCircleAll(randomPosition, 1f, obstacleLayer);
            if (colliders.Length == 0) // ถ้าไม่มีอะไรขวาง
            {
                return randomPosition;
            }
        }

        Debug.LogWarning("ไม่สามารถหาตำแหน่งใหม่ได้! ใช้ตำแหน่งเดิมแทน");
        return transform.position; // ถ้าหาตำแหน่งใหม่ไม่ได้ให้ใช้ที่เดิม
    }
}